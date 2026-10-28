using System.Diagnostics;

using SortsAndSearch.Searches;
using SortsAndSearch.Sorts;

namespace SortsAndSearch;

public class AlgorithmBenchmark
{
    public record ResultRecord
    {
        public string AlgorithmName { get; set; }
        public uint ArraySize { get; set; }
        public double ActualTimeMs { get; set; }
        public long TheoreticalOperations { get; set; }
    }
    
    public static void RunSorting(string name, int[] source, ISort sortMethod, List<ResultRecord> results, uint size)
    {
        int[] arr = (int[])source.Clone();
        long operations = 0;

        Stopwatch sw = Stopwatch.StartNew();
        sortMethod.Sort(arr, ref operations);
        sw.Stop();

        results.Add(new ResultRecord { AlgorithmName = name, ArraySize = size, ActualTimeMs = sw.Elapsed.TotalMilliseconds, TheoreticalOperations = operations });
        Console.WriteLine($"[Сортировка] {name,-15} | Время: {sw.Elapsed.TotalMilliseconds,8:F3} мс | Операций: {operations}");
    }
    
    public static void RunSearching(string name, ISearch searchMethod, int target, List<ResultRecord> results, uint size)
    {
        long operations = 0;

        Stopwatch sw = Stopwatch.StartNew();
        for (int i = 0; i < 100; i++)
        {
            operations = 0;
            searchMethod.Search(target, ref operations);
        }
        sw.Stop();

        double avgTimeMs = sw.Elapsed.TotalMilliseconds / 100.0;
        results.Add(new ResultRecord { AlgorithmName = name, ArraySize = size, ActualTimeMs = avgTimeMs, TheoreticalOperations = operations });
        Console.WriteLine($"[Поиск]      {name,-18} | Время: {avgTimeMs,8:F5} мс | Операций: {operations}");
    }

    private static void SaveToCsv(string filename, List<ResultRecord> records)
    {
        using var writer = new StreamWriter(filename);
        writer.WriteLine("Algorithm,Size,ActualTimeMs,TheoreticalOperations");
        foreach (var r in records)
        {
            writer.WriteLine($"{r.AlgorithmName},{r.ArraySize},{r.ActualTimeMs},{r.TheoreticalOperations}");
        }
    }
}