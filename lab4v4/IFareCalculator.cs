// IFareCalculator.cs
public interface IFareCalculator
{
    // Метод для обчислення вартості поїздки для N пасажирів
    decimal CalculateFare(int passengers);

    // Метод для обчислення середньої ціни поїздки
    decimal CalculateAverageFare(int passengers);
}
