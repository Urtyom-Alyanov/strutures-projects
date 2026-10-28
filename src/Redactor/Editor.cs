using Stacking;

public class Editor
{
    private readonly IMyStack<string> _undo = new LinkedStack<string>();
    private readonly IMyStack<string> _redo = new LinkedStack<string>();

    public string Text { get; private set; } = string.Empty;

    public void Apply(string text)
    {
        if (text == Text) return;
        _undo.Push(Text);
        _redo.Clear();
        Text = text;
    }

    public void Undo()
    {
        if (_undo.IsEmpty()) return;
        
        _redo.Push(Text);
        Text = _undo.Pop();
    }

    public void Redo()
    {
        if (_redo.IsEmpty()) return;
        _undo.Push(Text);
        Text = _redo.Pop();
    }

    public void ClearHistory()
    {
        _undo.Clear();
        _redo.Clear();
    }
}