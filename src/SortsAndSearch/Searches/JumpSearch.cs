namespace SortsAndSearch;

public class JumpSearch(int[] array) : ISearch
{
    public int Search(int target, ref long ops)
    {
        int n = array.Length;
        int step = (int)Math.Floor(Math.Sqrt(n));
        int prev = 0;
        while (prev < n && (++ops > 0 && array[Math.Min(step, n) - 1] < target))
        {
            prev = step;
            step += (int)Math.Floor(Math.Sqrt(n));
            if (prev >= n) return -1;
        }
        while (prev < n && (++ops > 0 && array[prev] < target))
        {
            prev++;
            if (prev == Math.Min(step, n)) return -1;
        }
        ops++;
        if (prev < n && array[prev] == target) return prev;
        return -1;
    }
}