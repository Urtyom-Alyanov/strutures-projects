using System.Diagnostics;

namespace Queue;

public class LinkedDeQueue<T>
{
    private record Node(T Value)
    {
        public Node? Next { get; set; }
        public Node? Previous { get; set; }
    };
    
    private Node? _head;
    private Node? _tail;
    public uint Size { get; private set; }
    
    public bool IsEmpty() => Size == 0;
    
    public void PushFront(T item)
    {
        Node newNode = new (item);
        if (IsEmpty())
            _head = _tail = newNode;
        else
        {
            newNode.Next = _head;
            _head?.Previous = newNode;
            _head = newNode;
        }
        Size++;
    }
    
    public void PushBack(T item)
    {
        Node newNode = new (item);
        if (IsEmpty())
            _head = _tail = newNode;
        else
        {
            newNode.Previous = _tail;
            _tail?.Next = newNode;
            _tail = newNode;
        }
        Size++;
    }
    
    public T PopFront()
    {
        if (IsEmpty()) throw new InvalidOperationException("Дек пуст.");
        Debug.Assert(_head != null, nameof(_head) + " != null");
        T val = _head.Value;
        _head = _head.Next;
        if (_head != null) _head.Previous = null;
        else _tail = null;
        Size--;
        return val;
    }

    public T PopBack()
    {
        if (IsEmpty()) throw new InvalidOperationException("Дек пуст.");
        Debug.Assert(_tail != null, nameof(_tail) + " != null");
        T val = _tail.Value;
        _tail = _tail.Previous;
        if (_tail != null) _tail.Next = null;
        else _head = null;
        Size--;
        return val;
    }
    
    public T PeekFront()
    {
        if (IsEmpty()) throw new InvalidOperationException("Дек пуст.");
        Debug.Assert(_head != null, nameof(_head) + " != null");
        return _head.Value;
    }

    public T PeekBack()
    {
        if (IsEmpty()) throw new InvalidOperationException("Дек пуст.");
        Debug.Assert(_tail != null, nameof(_tail) + " != null");
        return _tail.Value;
    }

    public void Clear()
    {
        _head = null;
        _tail = null;
        Size = 0;
    }
}