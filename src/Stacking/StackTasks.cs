namespace Stacking;

public static class StackTasks
{
    /// <summary>
    /// Определение палиндрома
    /// </summary>
    /// <param name="text">текст</param>
    /// <returns>Является ли он палиндромом?</returns>
    public static bool IsPalindrome(string text)
    {
        var cleanChars = new char[text.Length];
        int len = 0;
        
        foreach (var c in text.ToLower().Where(char.IsLetterOrDigit))
            cleanChars[len++] = c;

        IMyStack<char> stack = new ArrayStack<char>();
        
        for (int i = 0; i < len; i++)
            stack.Push(cleanChars[i]);
        
        for (int i = 0; i < len; i++)
            if (cleanChars[i] != stack.Pop())
                return false;
        
        return true;
    }

    /// <summary>
    /// Вычисление выражения в польской инверсной записи
    /// </summary>
    /// <param name="rpnExpression">Выражение в польской инверсной записи</param>
    /// <returns>Результат вычисления</returns>
    /// <exception cref="DivideByZeroException">Деление на 0</exception>
    /// <exception cref="ArgumentException">Неизвестный оператор</exception>
    public static double EvaluateRPN(string rpnExpression)
    {
        IMyStack<double> stack = new ArrayStack<double>();
        
        string[] tokens = rpnExpression.Split([' '], StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var token in tokens)
            if (double.TryParse(token, out double number))
                stack.Push(number);
            else
            {
                double b = stack.Pop();
                double a = stack.Pop();
                
                double result = token switch
                {
                    "+" => a + b,
                    "-" => a - b,
                    "*" => a * b,
                    "/" => b != 0 ? a / b : throw new DivideByZeroException("Деление на ноль"),
                    _ => throw new ArgumentException($"Неизвестный оператор: {token}")
                };
                stack.Push(result);
            }
        
        return stack.Pop();
    }
}