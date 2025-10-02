// FareService.cs
public class FareService
{
    private IFareCalculator fareCalculator;

    // Конструктор, що приймає реалізацію IFareCalculator
    public FareService(IFareCalculator fareCalculator)
    {
        this.fareCalculator = fareCalculator;
    }

    // Метод для обчислення загальної вартості поїздки
    public void DisplayFare(int passengers)
    {
        decimal fare = fareCalculator.CalculateFare(passengers);
        decimal averageFare = fareCalculator.CalculateAverageFare(passengers);
        
        Console.WriteLine($"Загальна вартість поїздки для {passengers} пасажирів: {fare:0.##} грн.");
        Console.WriteLine($"Середня вартість поїздки на одного пасажира: {averageFare:0.##} грн.");
    }
}
