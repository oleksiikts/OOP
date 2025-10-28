using System.Collections.Generic;
using System.Linq;

namespace lab5v8.Models
{
    public class Delivery
    {
        public int Id { get; set; }
        public string DriverName { get; set; }

        // --- Композиція ---
        // Delivery "має" список посилок.
        // Список ініціалізується в конструкторі і є частиною Delivery.
        public List<Package> Packages { get; private set; }

        public Delivery(int id, string driverName)
        {
            Id = id;
            DriverName = driverName;
            Packages = new List<Package>(); // Ініціалізація композиції
        }

        public void AddPackage(Package package)
        {
            if (package != null)
            {
                Packages.Add(package);
            }
        }

        // --- Обчислення 1: Сумарна маса ---
        public double GetTotalWeight()
        {
            return Packages.Sum(p => p.WeightKg);
        }

        // --- Обчислення 2: Вартість (база + за кг) ---
        public decimal CalculateTotalCost(decimal pricePerKg)
        {
            if (pricePerKg < 0)
            {
                throw new ArgumentException("Ціна за кілограм не може бути від'ємною.");
            }

            decimal baseCost = Packages.Sum(p => p.BasePrice);
            decimal weightCost = (decimal)GetTotalWeight() * pricePerKg;
            
            return baseCost + weightCost;
        }

        // --- Обчислення 3: SLA-відсоток вчасних ---
        public double GetSlaPercentage()
        {
            if (Packages.Count == 0)
            {
                // Якщо посилок немає, SLA вважається 100%
                return 100.0; 
            }

            int onTimeCount = Packages.Count(p => p.IsDeliveredOnTime);
            
            // (double)onTimeCount - приводимо до double для отримання дробового результату
            return (double)onTimeCount / Packages.Count * 100.0;
        }

        public override string ToString()
        {
            return $"Delivery [ID: {Id}, Driver: {DriverName}, Packages: {Packages.Count}]";
        }
    }
}