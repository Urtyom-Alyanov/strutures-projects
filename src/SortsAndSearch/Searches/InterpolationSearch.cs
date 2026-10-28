namespace SortsAndSearch.Searches;

public class InterpolationSearch(int[] array) : ISearch
{
    public int Search(int target, ref long ops)
    {
        int low = 0, high = array.Length - 1;
        while (low <= high && target >= array[low] && target <= array[high])
        {
            ops++;
            if (low == high)
            {
                if (array[low] == target) return low;
                return -1;
            }
            int pos = low + (((int)((double)(high - low) / (array[high] - array[low]) * (target - array[low]))));
            if (pos < low || pos > high) break;
            ops++;
            if (array[pos] == target) return pos;
            if (array[pos] < target) low = pos + 1;
            else high = pos - 1;
        }
        return -1;
    }
}