namespace TfIdf.Matrix;

/// <summary>
/// Симметричная сжатая матрица
/// </summary>
public class SymmetricSparseMatrix
{
    private readonly SparseMatrix _sparseMatrix;
    
    public uint Size => _sparseMatrix.Rows;
    public uint NonZeros => _sparseMatrix.NonZeros;
    public double[] Values => _sparseMatrix.Values;
    public uint[] ColIndices => _sparseMatrix.ColIndices;
    public uint[] RowPtr => _sparseMatrix.RowPtr;
    
    public SymmetricSparseMatrix(SparseMatrix sparseMatrix)
    {
        if (sparseMatrix.Rows != sparseMatrix.Cols)
            throw new ArgumentException("Матрица должна быть квадратной.");
        _sparseMatrix = sparseMatrix;
    }
    
    /// <summary>
    /// Создает симметричную матрицу из плотной, сохраняя только нижний треугольник в <paramref name="matrix"/>.
    /// </summary>
    /// <param name="matrix"></param>
    /// <returns></returns>
    public static SymmetricSparseMatrix FromDense(double[,] matrix)
    {
        uint size = (uint)matrix.GetLength(0);
        if (size != matrix.GetLength(1))
            throw new ArgumentException("Матрица должна быть квадратной.");

        var values = new List<double>();
        var colIndices = new List<uint>();
        var rowPtr = new uint[size + 1];

        for (uint i = 0; i < size; i++)
        {
            for (uint j = 0; j <= i; j++) // Только нижний треугольник
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
        return new (new (values.ToArray(), colIndices.ToArray(), rowPtr, size, size));
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
            if (row < col)
            {
                (row, col) = (col, row);
            }
            return _sparseMatrix[row, col];
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
        if (x.Length != Size)
            throw new ArgumentException($"Длина вектора ({x.Length}) должна совпадать с размером матрицы ({Size}).");

        var y = new double[Size];

        for (int i = 0; i < Size; i++)
        {
            uint start = RowPtr[i];
            uint end = RowPtr[i + 1];

            for (uint idx = start; idx < end; idx++)
            {
                uint j = ColIndices[idx];
                double val = Values[idx];

                y[i] += val * x[j];

                if (i != j)
                {
                    y[j] += val * x[i];
                }
            }
        }

        return y;
    }
    
    /// <summary>
    /// Преобразует матрицу в "плотную"
    /// </summary>
    /// <returns>Плотный формат матрицы</returns>
    public double[,] ToDense()
    {
        double[,] dense = _sparseMatrix.ToDense();
        
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < i; j++)
            {
                dense[j, i] = dense[i, j];
            }
        }
        
        return dense;
    }
    
    public override string ToString()
    {
        return $"SymmetricSparseMatrix (Size: {Size}x{Size}, NNZ (Lower): {NonZeros})\n" +
               $"  Values:      [{string.Join(", ", Values)}]\n" +
               $"  ColIndices:  [{string.Join(", ", ColIndices)}]\n" +
               $"  RowPtr:      [{string.Join(", ", RowPtr)}]";
    }
}