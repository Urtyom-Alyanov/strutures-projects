namespace Queue;

public class PriorityInsertionBufferQueue<T>(uint capacity = 4)
{
    private PriorityItem<T>[] _data = new PriorityItem<T>[capacity];
    public uint Size { get; private set; }
    
    public bool IsEmpty() => Size == 0;

    public void Enqueue(T value, int priority)
    {
        if (Size == _data.Length)
        {
            PriorityItem<T>[] newData = new PriorityItem<T>[_data.Length * 2];
            for (int i = 0; i < Size; i++) newData[i] = _data[i];
            _data = newData;
        }
        
        uint s = Size - 1;
        while (_data[s].Priority > priority)
        {
            _data[s + 1] = _data[s];
            s--;
        }
        _data[s + 1] = new PriorityItem<T>(value, priority);
        Size++;
    }

    public T Dequeue()
    {
        if (IsEmpty()) throw new InvalidOperationException("Очередь пуста.");
        T val = _data[0].Value;
        for (int i = 0; i < Size - 1; i++)
            _data[i] = _data[i + 1];
        _data[--Size] = default;
        return val;
    }
}