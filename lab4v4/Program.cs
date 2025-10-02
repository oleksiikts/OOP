using System;

class Program
{
    static void Main()
    {
        // Міський транспорт
        IFareCalculator cityFareCalculator = new CityTransportFareCalculator();
        FareService cityFareService = new FareService(cityFareCalculator);
        cityFareService.DisplayFare(5); // 5 пасажирів у міському транспорті

        Console.WriteLine();

        // Міжміський транспорт
        IFareCalculator intercityFareCalculator = new IntercityTransportFareCalculator();
        FareService intercityFareService = new FareService(intercityFareCalculator);
        intercityFareService.DisplayFare(5); // 5 пасажирів у міжміському транспорті
    }
}
