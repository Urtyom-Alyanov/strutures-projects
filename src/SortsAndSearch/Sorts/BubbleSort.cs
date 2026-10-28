namespace SortsAndSearch.Sorts;

public class BubbleSort : ISort
{
    public void Sort(int[] array, ref long ops)
    {
        int n = array.Length;
        for (int i = 0; i < n - 1; i++)
        for (int j = 0; j < n - i - 1; j++)
        {
            ops++;
            if (array[j] > array[j + 1])
                (array[j], array[j + 1]) = (array[j + 1], array[j]);
        }
    }
}