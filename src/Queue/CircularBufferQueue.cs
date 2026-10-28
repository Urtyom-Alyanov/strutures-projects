namespace Queue;

public class CircularBufferQueue<T>(uint capacity)
{
    private T[] _buffer = new T[capacity];
    private uint _head = 0;
    private uint _tail = 0;
    public uint Size { get; private set; }
    
    public bool IsEmpty() => Size == 0;
    public bool IsFull() => Size == capacity;
    
    public void EnqueueWithOverwrite(T item)
    {
        if (IsFull())
        {
            _buffer[_head] = item;
            _head = (_head + 1) % capacity;
        }
        else
        {
            _buffer[_tail] = item;
            _tail = (_tail + 1) % capacity;
            Size++;
        }
    }

    public T Dequeue()
    {
        if (IsEmpty()) throw new InvalidOperationException("Буфер пуст.");
        T item = _buffer[_head];
        _buffer[_head] = default(T)!;
        _head = (_head + 1) % capacity;
        Size--;
        return item;
    }
}