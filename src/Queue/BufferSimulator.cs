namespace Queue;

public class BufferSimulator
{
    public static void Simulate(uint bufferCapacity, uint totalTasksToGenerate)
    {
        var buffer = new CircularBufferQueue<string>(bufferCapacity);
        
        Console.WriteLine($"[Симулятор] Запуск. Емкость буфера: {bufferCapacity}");

        int generatedCount = 0;
        int processedCount = 0;
        int droppedCount = 0;

        while (generatedCount < totalTasksToGenerate)
        {
            for (int i = 0; i < 3; i++)
            {
                if (generatedCount < totalTasksToGenerate)
                {
                    generatedCount++;
                    string task = $"Task #{generatedCount}";
                    
                    if (buffer.IsFull())
                    {
                        droppedCount++;
                        Console.WriteLine($"  [ГЕНЕРАТОР] Буфер полон! Перезапись/вытеснение старой задачи ради {task}");
                    }
                    
                    buffer.EnqueueWithOverwrite(task);
                }
            }

            if (!buffer.IsEmpty())
            {
                string processedTask = buffer.Dequeue();
                processedCount++;
                Console.WriteLine($"[ОБРАБОТКА] Симулятор обработал: {processedTask}");
            }
        }

        Console.WriteLine($"\n[ИТОГИ СИМУЛЯЦИИ]");
        Console.WriteLine($"Всего сгенерировано: {generatedCount}");
        Console.WriteLine($"Успешно обработано: {processedCount}");
        Console.WriteLine($"Вытеснено/перезаписано из-за переполнения: {droppedCount}");
    }
}