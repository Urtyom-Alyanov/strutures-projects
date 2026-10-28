namespace SortsAndSearch.Sorts;

public class MergeSort : ISort
{
    public void Sort(int[] array, ref long ops) => SortInternal(array, 0, array.Length - 1, ref ops);
    
    private void SortInternal(int[] a, int l, int r, ref long ops)
    {
        if (l >= r)
            return;

        int m = l + (r - l) / 2;
        SortInternal(a, l, m, ref ops);
        SortInternal(a, m + 1, r, ref ops);
        Merge(a, l, m, r, ref ops);
    }
    private void Merge(int[] a, int l, int m, int r, ref long ops)
    {
        int n1 = m - l + 1;
        int n2 = r - m;
        int[] L = new int[n1]; int[] R = new int[n2];
        Array.Copy(a, l, L, 0, n1);
        Array.Copy(a, m + 1, R, 0, n2);
        int i = 0, j = 0, k = l;
        while (i < n1 && j < n2)
        {
            ops++;
            if (L[i] <= R[j]) a[k++] = L[i++];
            else a[k++] = R[j++];
        }
        while (i < n1) a[k++] = L[i++];
        while (j < n2) a[k++] = R[j++];
    }
}