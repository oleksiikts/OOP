using System;
using System.Threading;
using hw1.Cache;

namespace hw1
{
    // Допоміжний клас для демонстрації (відповідає обмеженню 'class')
    public class Report
    {
        public string? Title { get; set; }
        public override string ToString() => $"Report: '{Title}'";
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("--- Домашня робота 1: Узагальнений кеш ---");

            // 1. Створюємо кеш з максимальним розміром 3
            // Використовуємо <Report> як 'T', що задовольняє 'where T : class'
            var cache = new Cache<Report>(3);

            // 2. Додаємо елементи. Thread.Sleep(10) - для гарантованої різниці в часі
            cache.Add("report-A", new Report { Title = "Місячний звіт" });
            Thread.Sleep(10);
            cache.Add("report-C", new Report { Title = "Тижневий звіт" }); // Додаємо 'C' раніше 'B'
            Thread.Sleep(10);
            cache.Add("report-B", new Report { Title = "Денний звіт" });

            // 3. Дивимося вміст кешу. Все 3 елементи на місці.
            cache.DisplayCacheContents();

            // 4. Демонстрація алгоритму видалення
            // Додаємо 4-й елемент. Кеш переповнений (3/3).
            // "report-A" (найстаріший) має бути видалений.
            Console.WriteLine("\n-> Додавання 4-го елемента, щоб спрацював алгоритм видалення...");
            Thread.Sleep(10);
            cache.Add("report-D", new Report { Title = "Квартальний звіт" });
            
            // Дивимося вміст кешу. 'report-A' зник.
            cache.DisplayCacheContents();
            
            // 5. Демонстрація алгоритму сортування (без Linq.OrderBy/Sort)
            Console.WriteLine("\n--- Демонстрація сортування (Insertion Sort) ---");
            Console.WriteLine("Елементи, відсортовані за часом додавання (від найстарішого):");
            
            var sortedItems = cache.GetSortedByDate();
            
            foreach (var item in sortedItems)
            {
                Console.WriteLine($"  Value: {item.Value}, Added: {item.AddedAt:HH:mm:ss.fff}");
            }

            // 6. Демонстрація обмеження 'class'
            // Наступний рядок коду не скомпілюється, 
            // оскільки 'int' є 'struct', а не 'class'.
            // Cache<int> intCache = new Cache<int>(10); // Помилка CS0452
            
            Console.WriteLine("\n--- Демонстрація обмеження 'where T : class' ---");
            Console.WriteLine("Рядок 'Cache<int> intCache = ...' не скомпілюється, " +
                              "оскільки 'int' (struct) не відповідає обмеженню 'class'.");
        }
    }
}