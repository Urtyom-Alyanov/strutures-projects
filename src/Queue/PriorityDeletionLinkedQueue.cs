namespace Queue;

public class PriorityDeletionLinkedQueue<T>
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
        newNode.Next = _head;
        _head = newNode;
        Size++;
    }

    public T Dequeue()
    {
        if (IsEmpty()) throw new InvalidOperationException("Очередь пуста.");

        Node bestPrev = null;
        Node bestNode = _head;
        Node current = _head;
        Node prev = null;

        while (current != null)
        {
            if (current.Priority > bestNode.Priority)
            {
                bestNode = current;
                bestPrev = prev;
            }
            prev = current;
            current = current.Next;
        }

        T val = bestNode.Value;

        if (bestPrev == null)
            _head = _head.Next;
        else
            bestPrev.Next = bestNode.Next;
        Size--;

        return val;
    }
}