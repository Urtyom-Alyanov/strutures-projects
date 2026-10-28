namespace Stacking;

/// <summary>
/// Стек на динамическом массиве
/// </summary>
/// <param name="capacity">Вместимость</param>
/// <typeparam name="T">Тип данных в стеке</typeparam>
public class ArrayStack<T>(uint capacity = 4) : IMyStack<T>
{
    private T[] _data = new T[capacity];
    private int _top = 0;

    public bool IsEmpty() => _top == 0;

    public void Push(T item)
    {
        if (_top == _data.Length)
        {
            T[] newData = new T[_data.Length * 2];
            for (int i = 0; i < _data.Length; i++)
            {
                newData[i] = _data[i];
            }
            _data = newData;
        }
        _data[_top++] = item;
    }
    
    public T Peek()
    {
        return IsEmpty() ? throw new InvalidOperationException("Стек пуст.") : _data[_top - 1];
    }

    public T Pop()
    {
        if (IsEmpty()) throw new InvalidOperationException("Стек пуст.");
        T item = _data[--_top];
        _data[_top] = default(T)!;
        return item;
    }
    
    public void Clear()
    {
        for (int i = 0; i < _top; i++) _data[i] = default(T)!;
        _top = 0;
    }
}