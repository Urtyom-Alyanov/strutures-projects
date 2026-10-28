namespace Stacking;

public interface IMyStack<T>
{
    /// <summary>
    /// Положить на стек новый элемент
    /// </summary>
    /// <param name="item">элемент</param>
    void Push(T item);
    
    /// <summary>
    /// Взять и уменьшить стек
    /// </summary>
    /// <returns>элемент</returns>
    T Pop();
    
    /// <summary>
    /// Взять из стека
    /// </summary>
    /// <returns>элемент</returns>
    T Peek();
    
    /// <summary>
    /// Проверить стек на пустоту
    /// </summary>
    /// <returns>Пустой ли стек</returns>
    bool IsEmpty();
    
    /// <summary>
    /// Очистить стек
    /// </summary>
    void Clear();
}