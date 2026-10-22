use std::{
    cell::RefCell,
    panic::{AssertUnwindSafe, catch_unwind},
};

use crate::{
    compressed_sparse_matrix::CSR,
    error::{Result, Status, fail},
};

thread_local! {
    static LAST_ERROR: RefCell<String> = RefCell::new(String::new());
}

fn guard(f: impl FnOnce() -> Result<()>) -> u32 {
    let (status, msg) = match catch_unwind(AssertUnwindSafe(f)) {
        Ok(Ok(())) => return Status::Ok as u32,
        Ok(Err(e)) => (e.status, e.message),
        Err(_) => (Status::Panic, "panic in native code".to_owned()),
    };
    LAST_ERROR.with(|e| *e.borrow_mut() = msg);
    status as u32
}

unsafe fn slice<'a, T>(p: *const T, len: u32, name: &str) -> Result<&'a [T]> {
    if len == 0 {
        return Ok(&[]);
    }
    if p.is_null() {
        return fail(Status::NullPointer, format!("{name} is null"));
    }
    Ok(unsafe { std::slice::from_raw_parts(p, len as usize) })
}

unsafe fn slice_mut<'a, T>(p: *mut T, len: u32, name: &str) -> Result<&'a mut [T]> {
    if len == 0 {
        return Ok(&mut []);
    }
    if p.is_null() {
        return fail(Status::NullPointer, format!("{name} is null"));
    }
    Ok(unsafe { std::slice::from_raw_parts_mut(p, len as usize) })
}

unsafe fn handle<'a, T>(p: *const T) -> Result<&'a T> {
    match unsafe { p.as_ref() } {
        Some(r) => Ok(r),
        None => fail(Status::NullPointer, "null handle"),
    }
}

unsafe fn put<T>(out: *mut *mut T, v: T) -> Result<()> {
    if out.is_null() {
        return fail(Status::NullPointer, "out is null");
    }
    unsafe { *out = Box::into_raw(Box::new(v)) };
    Ok(())
}

unsafe fn write<T>(out: *mut T, v: T) -> Result<()> {
    if out.is_null() {
        return fail(Status::NullPointer, "out is null");
    }
    unsafe { *out = v };
    Ok(())
}

unsafe fn CSR_from_raw(
    row_count: usize,
    col_count: usize,
    idxptr: *const u32,
    idxptr_len: u32,
    indices: *const u32,
    data: *const f64,
    nnz: u32,
) -> Result<CSR> {
    CSR::new(
        row_count,
        col_count,
        unsafe { slice(idxptr, idxptr_len, "indidxptrptr") }?.to_vec(),
        unsafe { slice(indices, nnz, "indices") }?.to_vec(),
        unsafe { slice(data, nnz, "data") }?.to_vec(),
    )
}

// Ошибки

#[unsafe(no_mangle)]
pub unsafe extern "C" fn ai_last_error(buf: *mut u8, cap: u32) -> u32 {
    LAST_ERROR.with(|e| {
        let b = e.borrow();
        let b = b.as_bytes();
        if !buf.is_null() && cap > 0 {
            unsafe { std::ptr::copy_nonoverlapping(b.as_ptr(), buf, b.len().min(cap as usize)) };
        }
        b.len() as u32
    })
}

// Сжатая матрица

#[unsafe(no_mangle)]
pub unsafe extern "C" fn ai_CSR_from_coo(
    row_count: usize,
    col_count: usize,
    rows: *const u32,
    cols: *const u32,
    vals: *const f64,
    nnz: u32,
    out: *mut *mut CSR,
) -> u32 {
    guard(|| {
        let m = CSR::from_coo(
            row_count,
            col_count,
            unsafe { slice(rows, nnz, "rows") }?,
            unsafe { slice(cols, nnz, "cols") }?,
            unsafe { slice(vals, nnz, "vals") }?,
        )?;
        unsafe { put(out, m) }
    })
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn ai_CSR_from_arrays(
    row_count: usize,
    col_count: usize,
    idxptr: *const u32,
    idxptr_len: u32,
    indices: *const u32,
    data: *const f64,
    nnz: u32,
    out: *mut *mut CSR,
) -> u32 {
    guard(|| unsafe {
        put(
            out,
            CSR_from_raw(row_count, col_count, idxptr, idxptr_len, indices, data, nnz)?,
        )
    })
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn ai_CSR_free(p: *mut CSR) {
    if !p.is_null() {
        drop(unsafe { Box::from_raw(p) });
    }
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn ai_CSR_info(
    p: *const CSR,
    row_count: *mut u32,
    col_count: *mut u32,
    nnz: *mut u32,
) -> u32 {
    guard(|| unsafe {
        let m = handle(p)?;
        write(row_count, m.row_count().clone() as u32)?;
        write(col_count, m.col_count().clone() as u32)?;
        write(nnz, m.nnz() as u32)
    })
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn ai_CSR_spmv(
    p: *const CSR,
    x: *const f64,
    x_len: u32,
    y: *mut f64,
    y_len: u32,
) -> u32 {
    guard(|| {
        unsafe { handle(p) }?.spmv(unsafe { slice(x, x_len, "x") }?, unsafe {
            slice_mut(y, y_len, "y")
        }?)
    })
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn ai_CSR_get(p: *const CSR, row: usize, col: usize, out: *mut f64) -> u32 {
    guard(|| unsafe { write(out, handle(p)?.get(row, col)?) })
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn ai_CSR_transpose(p: *const CSR, out: *mut *mut CSR) -> u32 {
    guard(|| unsafe { put(out, handle(p)?.transpose()) })
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn ai_CSR_copy_to(
    p: *const CSR,
    idxptr: *mut u32,
    idxptr_len: u32,
    indices: *mut u32,
    indices_len: u32,
    data: *mut f64,
    data_len: u32,
) -> u32 {
    guard(|| {
        let m = unsafe { handle(p) }?;
        for (dst, src) in [(
            unsafe { slice_mut(idxptr, idxptr_len, "idxptr") }?,
            m.idxptr(),
        )] {
            if dst.len() != src.len() {
                return fail(Status::DimensionMismatch, "idxptr length");
            }
            dst.copy_from_slice(src);
        }
        let (di, dd) = (
            unsafe { slice_mut(indices, indices_len, "indices") }?,
            unsafe { slice_mut(data, data_len, "data") }?,
        );
        if di.len() != m.nnz() || dd.len() != m.nnz() {
            return fail(Status::DimensionMismatch, "indices/data length");
        }
        di.copy_from_slice(m.indices());
        dd.copy_from_slice(m.data());
        Ok(())
    })
}
