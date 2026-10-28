namespace SortsAndSearch;

public class LinearSearch(int[] array) : ISearch
{
    public int Search(int target, ref long ops)
    {
        for (int i = 0; i < array.Length; i++)
        {
            ops++;
            if (array[i] == target) return i;
        }
        return -1;
    }
}