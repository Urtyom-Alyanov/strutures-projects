namespace Queue;

public class CircularBufferQueue<T>(int capacity)
{
    private T[] _buffer = new T[capacity];
    private int _head = 0;
    private int _tail = 0;
    public int Size { get; private set; }
    
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