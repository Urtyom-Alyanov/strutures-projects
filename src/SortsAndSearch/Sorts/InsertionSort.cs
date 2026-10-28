namespace SortsAndSearch.Sorts;

public class InsertionSort : ISort
{
    public void Sort(int[] array, ref long ops)
    {
        for (int i = 1; i < array.Length; i++)
        {
            int key = array[i];
            int j = i - 1;
            while (j >= 0 && (++ops > 0 && array[j] > key))
            {
                array[j + 1] = array[j];
                j--;
            }
            array[j + 1] = key;
        }
    }
}