use crate::error::{Result, Status, check_len, fail};

#[derive(Debug, Clone)]
pub struct CSR {
    row_count: usize,
    col_count: usize,
    data: Vec<f64>,
    indices: Vec<u32>,
    idxptr: Vec<u32>,
}

impl CSR {
    pub fn row_count(&self) -> usize {
        self.row_count
    }
    pub fn col_count(&self) -> usize {
        self.col_count
    }
    pub fn nnz(&self) -> usize {
        self.data.len()
    }
    pub fn idxptr(&self) -> &[u32] {
        &self.idxptr
    }
    pub fn indices(&self) -> &[u32] {
        &self.indices
    }
    pub fn data(&self) -> &[f64] {
        &self.data
    }

    /// Создать экземпляр структуры разреженной матрицы, хранящайся как CSR
    pub fn new(
        row_count: usize,
        col_count: usize,
        idxptr: Vec<u32>,
        indices: Vec<u32>,
        data: Vec<f64>,
    ) -> Result<Self> {
        if row_count > usize::MAX || col_count > usize::MAX {
            return fail(Status::InvalidArgument, "dimensions exceed usize::MAX");
        }
        if idxptr.len() != row_count + 1 {
            return fail(Status::InvalidStructure, "indptr.len() must be nrows + 1");
        }
        if indices.len() != data.len() {
            return fail(Status::InvalidStructure, "indices and data lengths differ");
        }
        if idxptr[0] != 0 {
            return fail(Status::InvalidStructure, "indptr[0] must be 0");
        }
        if idxptr.windows(2).any(|w| w[0] > w[1]) {
            return fail(Status::InvalidStructure, "indptr must be non-decreasing");
        }
        if idxptr[row_count] as usize != indices.len() {
            return fail(Status::InvalidStructure, "indptr[nrows] must equal nnz");
        }
        for i in 0..row_count {
            let row = &indices[idxptr[i] as usize..idxptr[i + 1] as usize];
            let mut prev = -1i64;
            for &c in row {
                if c as usize >= row_count {
                    return fail(
                        Status::InvalidStructure,
                        format!("column {c} out of range in row {i}"),
                    );
                }
                if c as i64 <= prev {
                    return fail(
                        Status::InvalidStructure,
                        format!("columns must be strictly increasing in row {i}"),
                    );
                }
                prev = c as i64;
            }
        }
        Ok(Self {
            row_count,
            col_count,
            idxptr,
            indices,
            data,
        })
    }

    /// Преобразовать COO (координатный формат матрицы)
    pub fn from_coo(
        row_count: usize,
        col_count: usize,
        rows: &[u32],
        cols: &[u32],
        vals: &[f64],
    ) -> Result<Self> {
        check_len("cols", rows.len(), cols.len())?;
        check_len("vals", rows.len(), vals.len())?;

        if rows.len() > usize::MAX || row_count > usize::MAX || col_count > usize::MAX {
            return fail(Status::InvalidArgument, "size exceeds i32::MAX");
        }

        let mut t = Vec::with_capacity(rows.len());
        for k in 0..rows.len() {
            let (r, c) = (rows[k], cols[k]);
            if r as usize >= row_count || c as usize >= col_count {
                return fail(
                    Status::InvalidArgument,
                    format!("entry {k}: ({r}, {c}) out of bounds"),
                );
            }
            t.push((r, c, vals[k]));
        }
        t.sort_by_key(|&(r, c, _)| (r, c));

        let mut idxptr = vec![0u32; row_count + 1];
        let mut indices = Vec::with_capacity(t.len());
        let mut data: Vec<f64> = Vec::with_capacity(t.len());
        let mut last = None;
        for (r, c, v) in t {
            if last == Some((r, c)) {
                *data.last_mut().unwrap() += v;
            } else {
                indices.push(c);
                data.push(v);
                idxptr[r as usize + 1] += 1;
                last = Some((r, c));
            }
        }
        for i in 0..row_count {
            idxptr[i + 1] += idxptr[i];
        }

        Ok(Self {
            row_count,
            col_count,
            data,
            indices,
            idxptr,
        })
    }

    /// Получить строку матрицы i-того столбца,
    /// в первом элементе - индексы, во втором - соответствующие
    /// им данные.
    pub fn row(&self, i: usize) -> (&[u32], &[f64]) {
        let (s, e) = (self.idxptr[i] as usize, self.idxptr[i + 1] as usize);
        (&self.indices[s..e], &self.data[s..e])
    }

    /// Получить i-тый и j-тый элемент матрицы
    pub fn get(&self, i: usize, j: usize) -> Result<f64> {
        if i >= self.row_count || j >= self.col_count {
            return fail(
                Status::InvalidArgument,
                format!("index ({i}, {j}) out of bounds"),
            );
        }
        let (idx, val) = self.row(i);
        Ok(idx.binary_search(&(j as u32)).map_or(0.0, |k| val[k]))
    }

    /// Умножение матрицы на вектор `x` и получение из него
    /// вектора `y`
    pub fn spmv(&self, x: &[f64], y: &mut [f64]) -> Result<()> {
        check_len("x", self.col_count, x.len())?;
        check_len("y", self.row_count, y.len())?;
        for i in 0..self.row_count {
            let (idx, val) = self.row(i);
            y[i] = idx.iter().zip(val).map(|(&j, &v)| v * x[j as usize]).sum();
        }
        Ok(())
    }

    /// Перевернуть матрицу набок, таким образом можно получить
    /// из Compressed Sparse Row Compressed Sparse Column, то есть
    /// то же самое, но по столбцам
    pub fn transpose(&self) -> Self {
        let nnz = self.nnz();
        let mut idxptr = vec![0u32; self.row_count + 1];
        for &c in &self.indices {
            idxptr[c as usize + 1] += 1;
        }
        for i in 0..self.row_count {
            idxptr[i + 1] += idxptr[i];
        }
        let mut next = idxptr[..self.row_count].to_vec();
        let mut indices = vec![0u32; nnz];
        let mut data = vec![0.0; nnz];
        for r in 0..self.row_count {
            let (idx, val) = self.row(r);
            for (&c, &v) in idx.iter().zip(val) {
                let dst = next[c as usize] as usize;
                indices[dst] = r as u32;
                data[dst] = v;
                next[c as usize] += 1;
            }
        }
        Self {
            row_count: self.col_count,
            col_count: self.row_count,
            idxptr,
            indices,
            data,
        }
    }
}
