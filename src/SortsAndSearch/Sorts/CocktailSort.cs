namespace SortsAndSearch.Sorts;

public class CocktailSort : ISort
{
    public void Sort(int[] array, ref long ops)
    {
        bool swapped = true;
        int start = 0;
        int end = array.Length - 1;
        while (swapped)
        {
            swapped = false;
            for (int i = start; i < end; ++i)
            {
                ops++;
                if (array[i] <= array[i + 1])
                    continue;

                (array[i], array[i + 1]) = (array[i + 1], array[i]);
                swapped = true;
            }
            if (!swapped) break;
            swapped = false;
            end--;
            for (int i = end - 1; i >= start; --i)
            {
                ops++;
                if (array[i] <= array[i + 1])
                    continue;

                (array[i], array[i + 1]) = (array[i + 1], array[i]);
                swapped = true;
            }
            start++;
        }
    }
}