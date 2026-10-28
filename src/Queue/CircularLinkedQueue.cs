using System.Diagnostics;

namespace Queue;

public class CircularLinkedQueue<T>
{
    private record Node(T Value)
    {
        public Node? Next;
    }
    
    public uint Size { get; private set; }
    public bool IsEmpty() => Size == 0;

    private Node? _tail;
    
    public void Enqueue(T item)
    {
        Node newNode = new (item);
        if (_tail == null)
        {
            _tail = newNode;
            _tail.Next = _tail;
        }
        else
        {
            newNode.Next = _tail.Next;
            _tail.Next = newNode;
            _tail = newNode;
        }
        Size++;
    }

    public T Dequeue()
    {
        if (IsEmpty()) throw new InvalidOperationException("Очередь пуста.");
        Node head = _tail?.Next;
        T val = head.Value;

        if (_tail == head)
            _tail = null;
        else
            _tail.Next = head.Next;
        Size--;
        return val;
    }
}