namespace SortsAndSearch.Sorts;

public class QuickSort : ISort
{
    public void Sort(int[] array, ref long ops) => SortInternal(array, 0, array.Length - 1, ref ops);
    
    private static void SortInternal(int[] a, int low, int high, ref long ops)
    {
        if (low >= high)
            return;

        int pi = Partition(a, low, high, ref ops);
        SortInternal(a, low, pi - 1, ref ops);
        SortInternal(a, pi + 1, high, ref ops);
    }
    private static int Partition(int[] a, int low, int high, ref long ops)
    {
        int pivot = a[high];
        int i = (low - 1);
        for (int j = low; j <= high - 1; j++)
        {
            ops++;
            if (a[j] < pivot)
            {
                i++;
                (a[i], a[j]) = (a[j], a[i]);
            }
        }
        (a[i + 1], a[high]) = (a[high], a[i + 1]);
        return i + 1;
    }
}