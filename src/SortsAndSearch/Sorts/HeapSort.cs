namespace SortsAndSearch.Sorts;

public class HeapSort : ISort
{
    public void Sort(int[] array, ref long ops)
    {
        int n = array.Length;
        for (int i = n / 2 - 1; i >= 0; i--) Heapify(array, n, i, ref ops);
        for (int i = n - 1; i > 0; i--)
        {
            (array[0], array[i]) = (array[i], array[0]);
            Heapify(array, i, 0, ref ops);
        }
    }
    
    private void Heapify(int[] a, int n, int i, ref long ops)
    {
        int largest = i;
        int l = 2 * i + 1;
        int r = 2 * i + 2;
        if (l < n) { ops++; if (a[l] > a[largest]) largest = l; }
        if (r < n) { ops++; if (a[r] > a[largest]) largest = r; }

        if (largest == i)
            return;

        (a[i], a[largest]) = (a[largest], a[i]);
        Heapify(a, n, largest, ref ops);
    }
}