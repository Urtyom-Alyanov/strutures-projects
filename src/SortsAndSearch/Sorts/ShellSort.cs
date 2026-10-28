namespace SortsAndSearch.Sorts;

public class ShellSort : ISort
{
    public void Sort(int[] array, ref long ops)
    {
        int n = array.Length;
        for (int gap = n / 2; gap > 0; gap /= 2)
        {
            for (int i = gap; i < n; i++)
            {
                int temp = array[i];
                int j = i;
                while (j >= gap && (++ops > 0 && array[j - gap] > temp))
                {
                    array[j] = array[j - gap];
                    j -= gap;
                }
                array[j] = temp;
            }
        }
    }
}