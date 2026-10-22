using System.Text;

using AiEngine.Native.Interop;

namespace AiEngine.Native.Matrix;

public unsafe class CompressedSparseMatrix : IDisposable
{
    private bool _disposed;

    internal CSR* Handle { get; private set; }

    private CompressedSparseMatrix(CSR* handle)
    {
        if (handle == null)
            throw new ArgumentNullException(nameof(handle));

        Handle = handle;
    }

    #region Фабричные методы

    /// <summary>
    /// Меняет формат хранения матрицы с COO на CSR
    /// </summary>
    public static CompressedSparseMatrix FromCoo(nuint rowCount, nuint colCount, ReadOnlySpan<uint> rows, ReadOnlySpan<uint> cols, ReadOnlySpan<double> vals)
    {
        if (rows.Length != cols.Length || rows.Length != vals.Length)
            throw new ArgumentException("Длины массивов rows, cols и vals должны совпадать.");

        uint nnz = (uint)rows.Length;
        CSR* ptr = null;

        fixed (uint* pRows = rows)
        fixed (uint* pCols = cols)
        fixed (double* pVals = vals)
        {
            uint code = NativeMethods.ai_CSR_from_coo(rowCount, colCount, pRows, pCols, pVals, nnz, &ptr);
            AiEngineException.CheckResult(code);
        }

        return new CompressedSparseMatrix(ptr);
    }

    /// <summary>
    /// Создает матрицу CSR из плоских массивов (idxptr, indices, data).
    /// </summary>
    public static CompressedSparseMatrix FromArrays(nuint rowCount, nuint colCount, ReadOnlySpan<uint> idxptr, ReadOnlySpan<uint> indices, ReadOnlySpan<double> data)
    {
        if (indices.Length != data.Length)
            throw new ArgumentException("Длины массивов indices и data должны совпадать.");

        uint nnz = (uint)indices.Length;
        CSR* ptr = null;

        fixed (uint* pIdxptr = idxptr)
        fixed (uint* pIndices = indices)
        fixed (double* pData = data)
        {
            uint code = NativeMethods.ai_CSR_from_arrays(
                rowCount, colCount,
                pIdxptr, (uint)idxptr.Length,
                pIndices, pData, nnz,
                &ptr
            );
            AiEngineException.CheckResult(code);
        }

        return new CompressedSparseMatrix(ptr);
    }
    #endregion

    #region Управление памятью
    /// <summary>
    /// Освобождение памяти
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            _disposed = true;
            if (Handle != null)
            {
                NativeMethods.ai_CSR_free(Handle);
                Handle = null;
            }
        }
    }

    /// <summary>
    /// Копирование внутренних данных матрицы в выделенные массивы/буферы.
    /// </summary>
    public void CopyTo(Span<uint> idxptr, Span<uint> indices, Span<double> data)
    {
        EnsureNotDisposed();

        fixed (uint* pIdxptr = idxptr)
        fixed (uint* pIndices = indices)
        fixed (double* pData = data)
        {
            uint code = NativeMethods.ai_CSR_copy_to(
                Handle,
                pIdxptr, (uint)idxptr.Length,
                pIndices, (uint)indices.Length,
                pData, (uint)data.Length
            );
            AiEngineException.CheckResult(code);
        }
    }

    private void EnsureNotDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed || Handle == null, nameof(CompressedSparseMatrix));
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~CompressedSparseMatrix()
    {
        Dispose(false);
    }
    #endregion

    #region Информация о матрице
    /// <summary>
    /// Получение элемента по индексу
    /// </summary>
    public double this[nuint row, nuint col]
    {
        get
        {
            EnsureNotDisposed();
            double value = 0;
            uint code = NativeMethods.ai_CSR_get(Handle, row, col, &value);
            AiEngineException.CheckResult(code);
            return value;
        }
    }

    /// <summary>
    /// Возвращает количество строк, столбцов и количество ненулевых элементов (NNZ).
    /// </summary>
    public (uint RowCount, uint ColCount, uint Nnz) GetInfo()
    {
        EnsureNotDisposed();
        uint rows = 0, cols = 0, nnz = 0;
        uint code = NativeMethods.ai_CSR_info(Handle, &rows, &cols, &nnz);
        AiEngineException.CheckResult(code);
        return (rows, cols, nnz);
    }
    #endregion

    #region Действия с матрицей
    /// <summary>
    /// Транспонирование матрицы. Возвращает новую матрицу CSR.
    /// </summary>
    public CompressedSparseMatrix Transpose()
    {
        EnsureNotDisposed();
        CSR* outPtr = null;
        uint code = NativeMethods.ai_CSR_transpose(Handle, &outPtr);
        AiEngineException.CheckResult(code);
        return new CompressedSparseMatrix(outPtr);
    }

    /// <summary>
    /// Перемножение матрицы на вектор (SpMV): y = A * x.
    /// Происходит на выходе мутация <paramref name="y" />
    /// </summary>
    /// <param name="x">Вектор, на который умножаем</param>
    /// <param name="y">Выходной вектор</param>
    public void MultiplyVector(ReadOnlySpan<double> x, Span<double> y)
    {
        EnsureNotDisposed();

        fixed (double* pX = x)
        fixed (double* pY = y)
        {
            uint code = NativeMethods.ai_CSR_spmv(Handle, pX, (uint)x.Length, pY, (uint)y.Length);
            AiEngineException.CheckResult(code);
        }
    }

    /// <summary>
    /// Умножение матрицы на вектор с возвратом нового массива.
    /// </summary>
    public double[] MultiplyVector(ReadOnlySpan<double> x)
    {
        var (rowCount, _, _) = GetInfo();
        double[] y = new double[rowCount];
        MultiplyVector(x, y);
        return y;
    }
    #endregion
}