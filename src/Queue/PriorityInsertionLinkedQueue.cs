using System.Diagnostics;

namespace Queue;

public class PriorityInsertionLinkedQueue<T>
{
    private record Node(T Value, uint Priority)
    {
        public Node? Next;
    }

    private Node? _head;
    public uint Size { get; private set; }

    public bool IsEmpty() => Size == 0;

    public void Enqueue(T value, uint priority)
    {
        Node newNode = new (value, priority);
        if (_head == null || _head.Priority > priority)
        {
            newNode.Next = _head;
            _head = newNode;
        }
        else
        {
            Node current = _head;
            while (current.Next != null && current.Next.Priority <= priority)
            {
                current = current.Next;
            }
            newNode.Next = current.Next;
            current.Next = newNode;
        }
        Size++;
    }

    public T Dequeue()
    {
        if (IsEmpty()) throw new InvalidOperationException("Очередь пуста.");
        Debug.Assert(_head != null, nameof(_head) + " != null");
        T val = _head.Value;
        _head = _head.Next;
        Size--;
        return val;
    }
}