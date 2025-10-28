using System;
using System.Linq;
using lab5v8.Models;
using lab5v8.Models.Exceptions;
using lab5v8.Repository;

namespace lab5v8
{
    class Program
    {
        static void Main(string[] args)
        {
            // Встановлюємо кодування для коректного відображення в консолі
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("--- Лабораторна робота 5. Доставка (v8) ---");

            // 1. Створення репозиторію (Generics)
            // Ми створюємо конкретну реалізацію Repository<Delivery>
            IRepository<Delivery> deliveryRepo = new Repository<Delivery>();

            try
            {
                // 2. Створення сутностей
                // Створюємо посилки
                var p1 = new Package("PKG-001", 2.5, 10.00m, "Zone A", true);
                var p2 = new Package("PKG-002", 1.0, 5.50m, "Zone B", true);
                var p3 = new Package("PKG-003", 5.0, 15.00m, "Zone A", false); // Ця не вчасна
                var p4 = new Package("PKG-004", 0.5, 20.00m, "Zone C", true);
                var p5 = new Package("PKG-005", 3.0, 8.00m, "Zone B", true);

                // Створення доставок (Composition)
                var delivery1 = new Delivery(101, "Іван");
                delivery1.AddPackage(p1);
                delivery1.AddPackage(p2);
                delivery1.AddPackage(p3);

                var delivery2 = new Delivery(102, "Марія");
                delivery2.AddPackage(p4);
                delivery2.AddPackage(p5);
                
                // Додавання в репозиторій
                deliveryRepo.Add(delivery1);
                deliveryRepo.Add(delivery2);

                Console.WriteLine("\n--- Усі доставки додано до репозиторію ---");
                foreach (var d in deliveryRepo.All())
                {
                    Console.WriteLine(d);
                }

                // 3. Демонстрація обчислень
                Console.WriteLine($"\n--- Обчислення для доставки {delivery1.Id} ({delivery1.DriverName}) ---");
                Console.WriteLine($"Загальна вага: {delivery1.GetTotalWeight()} kg");
                Console.WriteLine($"SLA (вчасність): {delivery1.GetSlaPercentage():F2}%");
                
                decimal costPerKg = 1.5m; // Ціна за кг
                Console.WriteLine($"Загальна вартість (база + {costPerKg:C}/kg): {delivery1.CalculateTotalCost(costPerKg):C}");

                // 4. Демонстрація LINQ (GroupBy)
                // Згрупуємо *всі* посилки з *усіх* доставок по зоні
                Console.WriteLine("\n--- Групування всіх посилок по зоні (LINQ GroupBy) ---");
                
                // SelectMany "розгладжує" список списків
                var allPackages = deliveryRepo.All().SelectMany(d => d.Packages);
                
                var packagesByZone = allPackages.GroupBy(p => p.DestinationZone);

                foreach (var group in packagesByZone.OrderBy(g => g.Key)) // Сортуємо по зоні
                {
                    Console.WriteLine($"Zone: {group.Key} (Кількість: {group.Count()})");
                    foreach (var pkg in group)
                    {
                        Console.WriteLine($"  -> {pkg.Id} ({pkg.WeightKg}kg)");
                    }
                    Console.WriteLine($"  Total weight in zone: {group.Sum(p => p.WeightKg)}kg");
                }

                // 5. Демонстрація обробки винятків
                Console.WriteLine("\n--- Демонстрація обробки винятків (InvalidPackageException) ---");
                try
                {
                    Console.WriteLine("Спроба створити посилку з від'ємною вагою...");
                    var invalidPackage = new Package("PKG-INVALID", -5.0, 10.00m, "Zone F", true);
                }
                catch (InvalidPackageException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Помилка! Спіймано виняток: {ex.Message}");
                    Console.ResetColor();
                }
                
                Console.WriteLine("\n--- Демонстрація іншого винятку (від'ємна ціна) ---");
                try
                {
                    Console.WriteLine("Спроба створити посилку з від'ємною ціною...");
                    var p_invalid_price = new Package("PKG-BADPRICE", 1.0, -100.00m, "Zone G", true);
                }
                catch (InvalidPackageException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Помилка! Спіймано виняток: {ex.Message}");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                // Загальний catch-блок для інших неочікуваних помилок
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"Сталася неочікувана глобальна помилка: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}