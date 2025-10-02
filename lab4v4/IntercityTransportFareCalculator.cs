// IntercityTransportFareCalculator.cs
public class IntercityTransportFareCalculator : TransportFareCalculator
{
    // Конструктор міжміського транспорту, де базова вартість встановлюється
    public IntercityTransportFareCalculator() : base(10.00m) { }

    // Реалізація обчислення вартості поїздки для N пасажирів
    public override decimal CalculateFare(int passengers)
    {
        return baseFare * passengers * 1.2m; // Для міжміського транспорту вартість вища
    }
}
