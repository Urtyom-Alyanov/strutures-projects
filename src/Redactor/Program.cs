using System.Text;

namespace Redactor;

public enum Mode
{
    Normal,
    Insert
}

public class Program
{
    private static readonly Editor _editor = new();
    private static StringBuilder _currentBuffer = new (_editor.Text);
    private static Mode _currentMode = Mode.Normal;
    
    public static void Main(string[] args)
    {
        void RedrawScreen()
        {
            Console.Clear();
            Console.WriteLine($"--- RE:doctor --- [{_currentMode.ToString().ToUpper()}]");
            Console.WriteLine(_currentMode == Mode.Normal ? "[i] - Insert mode | [u] - Undo | [r] - Redo | [q] - Quit" : "Для выхода из режима редактирования нажмите ctrl+q");
            Console.WriteLine(new string('-', 40));
            Console.Write($"{_currentBuffer}");
        }
        
        while (true)
        {
            RedrawScreen();
            if (_currentMode == Mode.Normal)
            {
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.I)
                {
                    _currentMode = Mode.Insert;
                    _currentBuffer.Clear();
                    _currentBuffer.Append(_editor.Text);
                }
                else if (key.Key == ConsoleKey.U)
                {
                    _editor.Undo();
                    _currentBuffer.Clear();
                    _currentBuffer.Append(_editor.Text);
                }
                else if (key.Key == ConsoleKey.R)
                {
                    _editor.Redo();
                    _currentBuffer.Clear();
                    _currentBuffer.Append(_editor.Text);
                }
                else if (key.Key == ConsoleKey.Q)
                    break;
            }
            else if (_currentMode == Mode.Insert)
            {
                var key = Console.ReadKey(true);

                if (key.Modifiers.HasFlag(ConsoleModifiers.Control) && key.Key == ConsoleKey.Q)
                {
                    _editor.Apply(_currentBuffer.ToString());
                    _currentMode = Mode.Normal;
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (_currentBuffer.Length > 0)
                        _currentBuffer.Length--;
                }
                else if (key.Key == ConsoleKey.Enter)
                    _currentBuffer.Append("\n");
                else if (!char.IsControl(key.KeyChar))
                    _currentBuffer.Append(key.KeyChar);
            }
        }
    }
}