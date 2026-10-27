namespace TfIdf.Matrix;

/// <summary>
/// Формат разреженной матрицы COO
/// </summary>
public record SparseMatrixCOO
{
    public uint[] RowIdx;
    public uint[] ColIdx;
    public double[] Values;
    public uint Rows;
    public uint Cols;
}

/// <summary>
/// Разреженная матрица, хранящаяся в формате сжатых строк (CSR)
/// </summary>
public class SparseMatrix
{
    public double[] Values { get; }
    public uint[] ColIndices { get; }
    public uint[] RowPtr { get; }
    public uint Rows { get; }
    public uint Cols { get; }
    public uint NonZeros => (uint)Values.Length;
    
    public SparseMatrix(double[] values, uint[] colIndices, uint[] rowPtr, uint rows, uint cols)
    {
        Values = values ?? throw new ArgumentNullException(nameof(values));
        ColIndices = colIndices ?? throw new ArgumentNullException(nameof(colIndices));
        RowPtr = rowPtr ?? throw new ArgumentNullException(nameof(rowPtr));
        Rows = rows;
        Cols = cols;
    }
    
    /// <summary>
    /// Преобразовать формат плотной матрицы в разреженную
    /// </summary>
    /// <param name="matrix">Плотная матрица</param>
    /// <returns>Разреженная</returns>
    public static SparseMatrix FromDense(double[,] matrix)
    {
        uint rows = (uint)matrix.GetLength(0);
        uint cols = (uint)matrix.GetLength(1);

        var values = new System.Collections.Generic.List<double>();
        var colIndices = new System.Collections.Generic.List<uint>();
        var rowPtr = new uint[rows + 1];

        for (uint i = 0; i < rows; i++)
        {
            for (uint j = 0; j < cols; j++)
            {
                double val = matrix[i, j];
                if (val != 0.0)
                {
                    values.Add(val);
                    colIndices.Add(j);
                }
            }
            rowPtr[i + 1] = (uint)values.Count;
        }

        return new SparseMatrix(values.ToArray(), colIndices.ToArray(), rowPtr, rows, cols);
    }
    
    /// <summary>
    /// Преобразовать разреженную матрицу из формата COO в CSR
    /// </summary>
    /// <param name="coo">Матрица формата COO</param>
    /// <returns>Матрица формата CSR</returns>
    public static SparseMatrix FromCoo(SparseMatrixCOO coo)
    {
        int nnz = coo.Values.Length;
        var triplets = new (uint Row, uint Col, double Val)[nnz];
        
        for (int i = 0; i < nnz; i++)
        {
            triplets[i] = (coo.RowIdx[i], coo.ColIdx[i], coo.Values[i]);
        }

        Array.Sort(triplets, (a, b) => 
        {
            int cmp = a.Row.CompareTo(b.Row);
            return cmp != 0 ? cmp : a.Col.CompareTo(b.Col);
        });

        var csrValues = new double[nnz];
        var csrCols = new uint[nnz];
        var rowPtr = new uint[coo.Rows + 1];

        for (int i = 0; i < nnz; i++)
        {
            rowPtr[triplets[i].Row + 1]++;
            csrValues[i] = triplets[i].Val;
            csrCols[i] = triplets[i].Col;
        }

        for (int i = 0; i < coo.Rows; i++)
        {
            rowPtr[i + 1] += rowPtr[i];
        }

        return new SparseMatrix(csrValues, csrCols, rowPtr, coo.Rows, coo.Cols);
    }
    
    /// <summary>
    /// Доступ к элементу матрицы по индексам <paramref name="row"/> и <paramref name="col"/>
    /// </summary>
    /// <param name="row">Строка</param>
    /// <param name="col">Столбец</param>
    /// <exception cref="IndexOutOfRangeException">Индекс за пределами матрицы</exception>
    public double this[uint row, uint col]
    {
        get
        {
            if (row >= Rows || col >= Cols)
                throw new IndexOutOfRangeException("Индекс за пределами матрицы.");

            uint start = RowPtr[row];
            uint end = RowPtr[col + 1];

            for (uint i = start; i < end; i++)
            {
                if (ColIndices[i] == col)
                    return Values[i];
            }
            return 0.0;
        }
    }
    
    /// <summary>
    /// Умножение матрицы на вектор <paramref name="x"/>
    /// </summary>
    /// <param name="x">Вектор</param>
    /// <returns>Результат умножения</returns>
    /// <exception cref="ArgumentException">Длина вектора <paramref name="x"/> должна совпадать с количеством столбцов матрицы.</exception>
    public double[] Multiply(double[] x)
    {
        if (x.Length != Cols)
            throw new ArgumentException($"Длина вектора ({x.Length}) должна совпадать с количеством столбцов матрицы ({Cols}).");

        var y = new double[Rows];
        for (int i = 0; i < Rows; i++)
        {
            uint start = RowPtr[i];
            uint end = RowPtr[i + 1];
            double sum = 0.0;

            for (uint j = start; j < end; j++)
            {
                sum += Values[j] * x[ColIndices[j]];
            }
            y[i] = sum;
        }
        return y;
    }
    
    /// <summary>
    /// Преобразует матрицу в "плотную"
    /// </summary>
    /// <returns>Плотный формат матрицы</returns>
    public double[,] ToDense()
    {
        var dense = new double[Rows, Cols];
        for (int i = 0; i < Rows; i++)
        {   
            uint start = RowPtr[i];
            uint end = RowPtr[i + 1];
            for (uint j = start; j < end; j++)
            {
                dense[i, ColIndices[j]] = Values[j];
            }
        }
        return dense;
    }
    
    public override string ToString()
    {
        return $"SparseMatrix (Rows: {Rows}, Cols: {Cols}, NNZ: {NonZeros})\n" +
               $"  Values:      [{string.Join(", ", Values)}]\n" +
               $"  ColIndices:  [{string.Join(", ", ColIndices)}]\n" +
               $"  RowPtr:      [{string.Join(", ", RowPtr)}]";
    }
}