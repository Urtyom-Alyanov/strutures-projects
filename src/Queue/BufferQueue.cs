namespace Queue;

public class BufferQueue<T>(uint capacity)
{
    private T[] _data = new T[capacity];
    private uint _head = 0;
    private uint _tail;
    public uint Size { get; private set; } = 0;
    
    public bool IsEmpty() => Size == 0;
    
    public void Enqueue(T item)
    {
        if (Size == _data.Length)
        {
            T[] newData = new T[_data.Length * 2];
            for (int i = 0; i < Size; i++)
                newData[i] = _data[(_head + i) % _data.Length];
            _data = newData;
            _head = 0;
            _tail = Size;
        }
        _data[_tail] = item;
        _tail = (_tail + 1) % (uint)_data.Length;
        Size++;
    }

    public T Dequeue()
    {
        if (IsEmpty()) throw new InvalidOperationException("Очередь пуста.");
        T item = _data[_head];
        _data[_head] = default(T)!;
        _head = (_head + 1) % (uint)_data.Length;
        Size--;
        return item;
    }
}