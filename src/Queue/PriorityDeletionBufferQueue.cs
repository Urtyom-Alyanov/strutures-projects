namespace Queue;

public class PriorityDeletionBufferQueue<T>(uint capacity)
{
    private PriorityItem<T>[] _data =  new PriorityItem<T>[capacity];

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
        _data[Size++] = new PriorityItem<T>(value, priority);
    }

    public T Dequeue()
    {
        if (IsEmpty()) throw new InvalidOperationException("Очередь пуста.");

        int bestIndex = 0;
        for (int i = 1; i < Size; i++)
        {
            if (_data[i].Priority > _data[bestIndex].Priority)
            {
                bestIndex = i;
            }
        }

        T val = _data[bestIndex].Value;

        for (int i = bestIndex; i < Size - 1; i++)
        {
            _data[i] = _data[i + 1];
        }
        _data[--Size] = default;

        return val;
    }
}