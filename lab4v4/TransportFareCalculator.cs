// TransportFareCalculator.cs
public abstract class TransportFareCalculator : IFareCalculator
{
    // Базова вартість поїздки
    protected decimal baseFare;

    // Конструктор для ініціалізації базової вартості
    protected TransportFareCalculator(decimal baseFare)
    {
        this.baseFare = baseFare;
    }

    // Абстрактний метод для реалізації у похідних класах
    public abstract decimal CalculateFare(int passengers);

    // Реалізація методу CalculateAverageFare в базовому класі
    public decimal CalculateAverageFare(int passengers)
    {
        return CalculateFare(passengers) / passengers;
    }
}
