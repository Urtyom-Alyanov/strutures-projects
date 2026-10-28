namespace Queue;

public class SlidingWindow
{
    public static void FindMinMaxSlidingWindow(int[] array, int windowSize, out int[] minResults, out int[] maxResults)
    {
        int n = array.Length;
        if (n == 0 || windowSize <= 0 || windowSize > n)
        {
            minResults = new int[0];
            maxResults = new int[0];
            return;
        }

        int resultCount = n - windowSize + 1;
        minResults = new int[resultCount];
        maxResults = new int[resultCount];

        LinkedDeQueue<int> maxDeque = new ();
        LinkedDeQueue<int> minDeque = new ();

        int resIndex = 0;
        for (int i = 0; i < n; i++)
        {
            if (!maxDeque.IsEmpty() && maxDeque.PeekFront() <= i - windowSize)
                maxDeque.PopFront();
            if (!minDeque.IsEmpty() && minDeque.PeekFront() <= i - windowSize)
                minDeque.PopFront();
            
            while (!maxDeque.IsEmpty() && array[maxDeque.PeekBack()] <= array[i])
                maxDeque.PopBack();
            maxDeque.PushBack(i);
            
            while (!minDeque.IsEmpty() && array[minDeque.PeekBack()] >= array[i])
                minDeque.PopBack();
            
            minDeque.PushBack(i);

            if (i < windowSize - 1)
                continue;

            maxResults[resIndex] = array[maxDeque.PeekFront()];
            minResults[resIndex] = array[minDeque.PeekFront()];
            resIndex++;
        }
    }
}