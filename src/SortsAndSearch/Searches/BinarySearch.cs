namespace SortsAndSearch.Searches;

public class BinarySearch(int[] array) : ISearch
{
    public int Search(int target, ref long ops)
    {
        int l = 0, r = array.Length - 1;
        while (l <= r)
        {
            ops++;
            int m = l + (r - l) / 2;
            if (array[m] == target) return m;
            if (array[m] < target) l = m + 1;
            else r = m - 1;
        }
        return -1;
    }
}