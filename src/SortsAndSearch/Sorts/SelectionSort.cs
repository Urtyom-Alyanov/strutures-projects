namespace SortsAndSearch.Sorts;

public class SelectionSort : ISort
{
    public void Sort(int[] array, ref long ops)
    {
        int n = array.Length;
        for (int i = 0; i < n - 1; i++)
        {
            int minIdx = i;
            for (int j = i + 1; j < n; j++)
            {
                ops++;
                if (array[j] < array[minIdx]) minIdx = j;
            }
            (array[minIdx], array[i]) = (array[i], array[minIdx]);
        }
    }
}