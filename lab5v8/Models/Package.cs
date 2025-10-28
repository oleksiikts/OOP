using lab5v8.Models.Exceptions;

namespace lab5v8.Models
{
    public class Package
    {
        private double _weightKg;
        private decimal _basePrice;

        public string Id { get; set; }
        public string DestinationZone { get; set; }
        public bool IsDeliveredOnTime { get; set; }

        public double WeightKg
        {
            get => _weightKg;
            set
            {
                if (value <= 0)
                {
                    // Контроль вхідних даних + власний виняток
                    throw new InvalidPackageException($"Вага не може бути нульовою або від'ємною: {value}");
                }
                _weightKg = value;
            }
        }

        public decimal BasePrice
        {
            get => _basePrice;
            set
            {
                if (value < 0)
                {
                    // Контроль вхідних даних + власний виняток
                    throw new InvalidPackageException($"Базова ціна не може бути від'ємною: {value}");
                }
                _basePrice = value;
            }
        }

        // Конструктор для зручної ініціалізації
        public Package(string id, double weightKg, decimal basePrice, string zone, bool onTime)
        {
            Id = id;
            WeightKg = weightKg; // Тут спрацює валідація
            BasePrice = basePrice; // І тут
            DestinationZone = zone;
            IsDeliveredOnTime = onTime;
        }

        public override string ToString()
        {
            return $"Package [ID: {Id}, Weight: {WeightKg}kg, Price: {BasePrice:C}, Zone: {DestinationZone}, OnTime: {IsDeliveredOnTime}]";
        }
    }
}