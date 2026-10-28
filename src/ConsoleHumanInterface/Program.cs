using Queue;
using SortsAndSearch;
using SortsAndSearch.Searches;
using SortsAndSearch.Sorts;
using Stacking;
using TfIdf;
using TwoBalls;

class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Меню ===");
            Console.WriteLine("1) Стек: проверка палиндрома");
            Console.WriteLine("2) Стек: вычисление выражения в ОПЗ (RPN)");
            Console.WriteLine("3) Задача о двух стеклянных шариках");
            Console.WriteLine("4) Очередь: симулятор кольцевого буфера");
            Console.WriteLine("5) Сортировки и поиск (бенчмарк)");
            Console.WriteLine("6) TF-IDF поиск по документам");
            Console.WriteLine("0) Выход");
            Console.Write("\nВыберите пункт: ");

            string? choice = Console.ReadLine();
            Console.WriteLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        RunPalindrome();
                        break;
                    case "2":
                        RunRpn();
                        break;
                    case "3":
                        RunTwoEggs();
                        break;
                    case "4":
                        RunQueueSimulator();
                        break;
                    case "5":
                        RunSortsSearchBenchmark();
                        break;
                    case "6":
                        RunTfIdfSearch();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
            Console.ReadKey();
        }
    }

    static void RunPalindrome()
    {
        Console.Write("Введите строку для проверки: ");
        string? text = Console.ReadLine() ?? string.Empty;
        bool result = StackTasks.IsPalindrome(text);
        Console.WriteLine(result ? "Это палиндром." : "Это не палиндром.");
    }

    static void RunRpn()
    {
        Console.WriteLine("Введите выражение в обратной польской записи (числа и операторы + - * / через пробел).");
        Console.Write("Пример: 3 4 + 2 *: ");
        string? expr = Console.ReadLine() ?? string.Empty;
        double res = StackTasks.EvaluateRPN(expr);
        Console.WriteLine($"Результат: {res}");
    }

    static void RunTwoEggs()
    {
        Console.Write("Введите общее количество этажей: ");
        if (!int.TryParse(Console.ReadLine(), out int floors))
        {
            Console.WriteLine("Неверное число.");
            return;
        }

        Console.Write("Введите критический этаж (этаж, с которого шарик разбивается): ");
        if (!int.TryParse(Console.ReadLine(), out int critical))
        {
            Console.WriteLine("Неверное число.");
            return;
        }

        int steps = TwoEggsProblem.FindMinSteps(floors);
        int foundFloor = TwoEggsProblem.FindCriticalFloor(floors, critical);
        Console.WriteLine($"Минимальное количество бросков в худшем случае: {steps}");
        Console.WriteLine($"Найденный критический этаж при поиске: {foundFloor}");
    }

    static void RunQueueSimulator()
    {
        Console.Write("Введите ёмкость кольцевого буфера: ");
        if (!uint.TryParse(Console.ReadLine(), out uint capacity))
        {
            Console.WriteLine("Неверное число.");
            return;
        }

        Console.Write("Введите общее количество задач для генерации: ");
        if (!uint.TryParse(Console.ReadLine(), out uint total))
        {
            Console.WriteLine("Неверное число.");
            return;
        }

        BufferSimulator.Simulate(capacity, total);
    }

    static void RunSortsSearchBenchmark()
    {
        Console.Write("Введите размер массива для сортировки/поиска: ");
        if (!uint.TryParse(Console.ReadLine(), out uint size))
        {
            Console.WriteLine("Неверное число.");
            return;
        }

        int[] arr = new int[size];
        Random rnd = new Random();
        for (int i = 0; i < arr.Length; i++)
            arr[i] = rnd.Next(1, 100000);

        var results = new List<AlgorithmBenchmark.ResultRecord>();
        Console.WriteLine("\n=== Сортировки ===");

        AlgorithmBenchmark.RunSorting("BubbleSort", arr, new BubbleSort(), results, size);
        AlgorithmBenchmark.RunSorting("InsertionSort", arr, new InsertionSort(), results, size);
        AlgorithmBenchmark.RunSorting("SelectionSort", arr, new SelectionSort(), results, size);
        AlgorithmBenchmark.RunSorting("MergeSort", arr, new MergeSort(), results, size);
        AlgorithmBenchmark.RunSorting("QuickSort", arr, new QuickSort(), results, size);

        int[] sortedArr = (int[])arr.Clone();
        Array.Sort(sortedArr);

        Console.WriteLine("\n=== Поиск ===");
        int target = sortedArr[sortedArr.Length / 2];
        AlgorithmBenchmark.RunSearching("LinearSearch", new LinearSearch(sortedArr), target, results, size);
        AlgorithmBenchmark.RunSearching("BinarySearch", new BinarySearch(sortedArr), target, results, size);
    }

    static void RunTfIdfSearch()
    {
        Console.WriteLine("Введите документы по одному. Пустая строка - завершить ввод.");
        var documents = new List<string>();
        int idx = 1;
        while (true)
        {
            Console.Write($"Документ {idx}: ");
            string? line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;
            documents.Add(line);
            idx++;
        }

        if (documents.Count == 0)
        {
            Console.WriteLine("Нет документов для индексации.");
            return;
        }

        var engine = new SearchEngine();
        engine.FitTransform(documents);

        Console.WriteLine($"\nПроиндексировано {documents.Count} документов. Введите запрос (пустая строка - выход).");
        while (true)
        {
            Console.Write("\nЗапрос: ");
            string? query = Console.ReadLine();
            if (string.IsNullOrEmpty(query))
                break;

            var ranked = engine.RankDocuments(query);
            Console.WriteLine("Результаты:");
            for (int i = 0; i < ranked.Count; i++)
            {
                var r = ranked[i];
                Console.WriteLine($"  {i + 1}. Документ {r.DocIndex + 1} (оценка: {r.Score:F4})");
            }
        }
    }
}
