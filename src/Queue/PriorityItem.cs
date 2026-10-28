namespace Queue;

public struct PriorityItem<T>(T val, int p)
{
    public T Value = val;
    public int Priority = p;
}
