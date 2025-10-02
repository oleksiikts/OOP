// CityTransportFareCalculator.cs
public class CityTransportFareCalculator : TransportFareCalculator
{
    // Конструктор міського транспорту, де базова вартість встановлюється
    public CityTransportFareCalculator() : base(2.50m) { }

    // Реалізація обчислення вартості поїздки для N пасажирів
    public override decimal CalculateFare(int passengers)
    {
        return baseFare * passengers; // Для міського транспорту вартість залежить від кількості пасажирів
    }
}
