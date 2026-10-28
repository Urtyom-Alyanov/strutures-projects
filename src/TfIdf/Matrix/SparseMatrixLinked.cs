namespace TfIdf.Matrix;

public class SparseMatrixLinked(uint Rows, uint Cols)
{
    private record Node(uint Col, double value, Node? next)
    {
        public Node? Next { get; set; } = next;
        public double Value { get; set; } = value;
    }

    private readonly Node?[] _rowHeads =  new Node[Rows];
    
    public uint NonZeros 
    {
        get
        {
            uint count = 0;
            for (uint i = 0; i < Rows; i++)
            {
                Node? current = _rowHeads[i];
                while (current != null)
                {
                    count++;
                    current = current.Next;
                }
            }
            return count;
        }
    }
    
    /// <summary>
    /// Преобразовать формат плотной матрицы в разреженную
    /// </summary>
    /// <param name="matrix">Плотная матрица</param>
    /// <returns>Разреженная</returns>
    public static SparseMatrixLinked FromDense(double[,] matrix)
    {
        uint rows = (uint)matrix.GetLength(0);
        uint cols = (uint)matrix.GetLength(1);

        var sparseList = new SparseMatrixLinked(rows, cols);

        for (uint i = 0; i < rows; i++)
        {
            Node? tail = null;
            for (uint j = 0; j < cols; j++)
            {
                double val = matrix[i, j];
                if (val != 0.0)
                {
                    var newNode = new Node(j, val, null);
                    if (sparseList._rowHeads[i] == null)
                    {
                        sparseList._rowHeads[i] = newNode;
                    }
                    else
                    {
                        tail!.Next = newNode;
                    }
                    tail = newNode;
                }
            }
        }

        return sparseList;
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

            Node? current = _rowHeads[row];
            while (current != null)
            {
                if (current.Col == col)
                    return current.Value;
                if (current.Col > col)
                    break;
                current = current.Next;
            }
            return 0.0;
        }
        set
        {
            if (row >= Rows || col >= Cols)
                throw new IndexOutOfRangeException("Индекс за пределами матрицы.");

            Node? current = _rowHeads[row];
            Node? prev = null;

            while (current != null && current.Col < col)
            {
                prev = current;
                current = current.Next;
            }

            if (current != null && current.Col == col)
            {
                if (value == 0.0)
                {
                    if (prev == null) _rowHeads[row] = current.Next;
                    else prev.Next = current.Next;
                }
                else
                    current.Value = value;
                return;
            }

            if (value == 0.0)
                return;

            var newNode = new Node(col, value, current);
            if (prev == null) _rowHeads[row] = newNode;
            else prev.Next = newNode;
        }
    }
    
    /// <summary>
    /// Преобразование матрицы на связных списках в матрицу на буффере
    /// </summary>
    /// <returns>Матрица на векторах в формате CSR</returns>
    public SparseMatrix ToBufferSparseMatrix()
    {
        uint nnz = NonZeros;
        var values = new double[nnz];
        var colIndices = new uint[nnz];
        var rowPtr = new uint[Rows + 1];

        uint valIndex = 0;
        for (uint i = 0; i < Rows; i++)
        {
            rowPtr[i] = valIndex;
            Node? current = _rowHeads[i];
            while (current != null)
            {
                values[valIndex] = current.Value;
                colIndices[valIndex] = current.Col;
                valIndex++;
                current = current.Next;
            }
        }
        rowPtr[Rows] = valIndex;

        return new SparseMatrix(values, colIndices, rowPtr, Rows, Cols);
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
        for (uint i = 0; i < Rows; i++)
        {
            double sum = 0.0;
            Node? current = _rowHeads[i];
            while (current != null)
            {
                sum += current.Value * x[current.Col];
                current = current.Next;
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
        for (uint i = 0; i < Rows; i++)
        {
            Node? current = _rowHeads[i];
            while (current != null)
            {
                dense[i, current.Col] = current.Value;
                current = current.Next;
            }
        }
        return dense;
    }
}