using System.Diagnostics;

namespace Stacking;

/// <summary>
/// Стек на связных списках
/// </summary>
/// <typeparam name="T">Тип элемента</typeparam>
public class LinkedStack<T> : IMyStack<T>
{
    private record Node(T Value, Node? Next);
    
    private Node? _top;
    
    public bool IsEmpty() => _top == null;
    
    public void Push(T item)
    {
        _top = new Node(item, _top);
    }

    public T Pop()
    {
        if (IsEmpty()) throw new InvalidOperationException("Стек пуст.");
        Debug.Assert(_top != null, nameof(_top) + " != null");
        T value = _top.Value;
        _top = _top?.Next;
        return value;
    }

    public T Peek()
    {
        if (IsEmpty()) throw new InvalidOperationException("Стек пуст.");
        Debug.Assert(_top != null, nameof(_top) + " != null");
        return _top.Value;
    }

    public void Clear()
    {
        _top = null;
    }
}