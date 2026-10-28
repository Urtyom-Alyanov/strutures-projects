namespace TwoBalls;

public class TwoEggsProblem
{
    /// <summary>
    /// Находит минимальное количество шагов для <paramref name="totalFloors"/> этажей
    /// </summary>
    /// <param name="totalFloors">Сколько этажей в доме</param>
    /// <returns>Минимальное количество шагов</returns>
    public static int FindMinSteps(int totalFloors)
    {
        int x = 0;
        int sum = 0;
        while (sum < totalFloors)
        {
            x++;
            sum += x;
        }
        return x;
    }
    
    /// <summary>
    /// Ищет критичный этаж
    /// </summary>
    /// <param name="totalFloors">Общее количество этажей</param>
    /// <param name="criticalFloor">Критический этаж, где шарик разбивается</param>
    /// <returns>Номер критического этажа</returns>
    public static int FindCriticalFloor(int totalFloors, int criticalFloor)
    {
        int stepsLimit = FindMinSteps(totalFloors);
        int currentFloor = 0;
        int stepSize = stepsLimit;

        int previousFloor = 0;
        
        // 1. Ищем первый этаж
        while (currentFloor < totalFloors)
        {
            previousFloor = currentFloor;
            currentFloor += stepSize;
            if (currentFloor > totalFloors) currentFloor = totalFloors;

            if (currentFloor >= criticalFloor)
            {
                break;
            }

            stepSize--;
            if (stepSize <= 0) break;
        }

        // 2. Проверяем второй шарик линейно между предыдущим безопасным и текущим
        for (int f = previousFloor + 1; f < currentFloor; f++)
        {
            if (f >= criticalFloor)
            {
                return f;
            }
        }

        return currentFloor;
    }
}