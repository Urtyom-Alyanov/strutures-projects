using System.Diagnostics;

namespace Queue;

public class LinkedQueue<T>
{
    private record Node(T Value)
    {
        public Node? Next;
    }

    private Node? _head;
    private Node? _tail;
    public uint Size { get; private set; }

    public bool IsEmpty() => Size == 0;

    public void Enqueue(T item)
    {
        Node newNode = new Node(item);
        if (_tail == null)
            _head = _tail = newNode;
        else
        {
            _tail.Next = newNode;
            _tail = newNode;
        }
        Size++;
    }

    public T Dequeue()
    {
        if (IsEmpty()) throw new InvalidOperationException("Очередь пуста.");
        Debug.Assert(_head != null, nameof(_head) + " != null");
        T val = _head.Value;
        _head = _head.Next;
        if (_head == null) _tail = null;
        Size--;
        return val;
    }
}