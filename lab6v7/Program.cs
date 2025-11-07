// File: Program.cs

using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab6_Delegates
{
    // --- Критерій: Реалізація базових делегатів (2 бали) ---
    // 1. Власний делегат для арифметичних операцій
    public delegate double ArithmeticOperation(double a, double b);

    // --- Критерій: Комбіновані делегати (Бонус +1 бал) ---
    // 1. Власний делегат для системи сповіщень
    public delegate void WeatherAlertHandler(string message);


    class Program
    {
        // --- Методи для демонстрації комбінованого делегата ---
        public static void LogToConsole(string message)
        {
            Console.WriteLine($"[CONSOLE LOG]: {message}");
        }

        public static void SendSmsAlert(string message)
        {
            Console.WriteLine($"[SMS > +380...]: Отримано сповіщення: {message}");
        }
        // ---------------------------------------------------


        static void Main(string[] args)
        {
            Console.WriteLine("--- Лабораторна робота №6: Максимальний бал ---");

            // Створюємо колекцію об'єктів List<T>
            var weeklyReport = new List<TemperatureRecord>
            {
                new TemperatureRecord("Понеділок", 18.5),
                new TemperatureRecord("Вівторок", 22.0),
                new TemperatureRecord("Середа", 26.1),
                new TemperatureRecord("Четвер", 28.3),
                new TemperatureRecord("П'ятниця", 24.9),
                new TemperatureRecord("Субота", 30.5),
                new TemperatureRecord("Неділя", 21.0)
            };

            // --- Критерій: Використання анонімних методів (1 бал) ---
            Console.WriteLine("\n--- 1. Анонімний метод ---");
            // Присвоюємо реалізацію через анонімний метод
            ArithmeticOperation findAverage = delegate(double x, double y)
            {
                return (x + y) / 2;
            };
            Console.WriteLine($"Середня t° за Пн та Вт: {findAverage(18.5, 22.0):F1}°C");

            // --- Критерій: Реалізація ... лямбда-виразів (2 бали) ---
            Console.WriteLine("\n--- 2. Лямбда-вираз ---");
            // Той самий делегат, але з лямбда-виразом
            ArithmeticOperation findDifference = (x, y) => x - y;
            Console.WriteLine($"Різниця t° Чт та Пт: {findDifference(28.3, 24.9):F1}°C");

            // --- Критерій: Вбудовані делегати (Func, Action, Predicate) (1 бал) ---
            Console.WriteLine("\n--- 3. Вбудовані делегати (Варіант 7) ---");

            // Func<double, bool> - приймає double, повертає bool
            Func<double, bool> isHotCheck = temp => temp > 25.0;

            // Action<double> - приймає double, нічого не повертає (void)
            Action<double> printHotMessage = temp => Console.WriteLine($"Спекотно: {temp}°C");
            
            Console.WriteLine("Перевірка на спекотні дні (> 25°C):");
            foreach (var record in weeklyReport)
            {
                if (isHotCheck(record.DegreesC)) // Виклик Func
                {
                    printHotMessage(record.DegreesC); // Виклик Action
                }
            }
            
            // Predicate<T> - спеціалізований Func<T, bool> для перевірки умови
            Predicate<TemperatureRecord> isComfortable = r => r.DegreesC >= 20.0 && r.DegreesC <= 25.0;
            var comfortableDay = weeklyReport.Find(isComfortable);
            Console.WriteLine($"\nПерший комфортний день (Predicate): {comfortableDay}");


            // --- Критерій: Робота з колекціями й LINQ (1 бал) ---
            Console.WriteLine("\n--- 4. Обробка колекції (LINQ) ---");

            // Where (фільтрація): відбір днів із температурою > 25°C
            var hotDays = weeklyReport.Where(r => isHotCheck(r.DegreesC)); // Можна використати Func
            Console.WriteLine("\nСпекотні дні (LINQ Where):");
            foreach (var day in hotDays) Console.WriteLine(day);

            // Select (проекція): переведення у Фаренгейт
            var tempsInFahrenheit = weeklyReport.Select(r => $"{r.Day}: {(r.DegreesC * 9 / 5) + 32:F1}°F");
            Console.WriteLine("\nТемператури у Фаренгейтах (LINQ Select):");
            foreach (var s in tempsInFahrenheit) Console.WriteLine(s);

            // OrderBy (сортування): від найхолоднішого до найтеплішого
            var sortedDays = weeklyReport.OrderBy(r => r.DegreesC);
            Console.WriteLine("\nСортування (LINQ OrderBy):");
            foreach (var day in sortedDays) Console.WriteLine(day);

            // Aggregate (агрегація): пошук найвищої температури
            double maxTemp = weeklyReport.Aggregate(
                Double.MinValue, // початкове значення
                (max, next) => (next.DegreesC > max) ? next.DegreesC : max
            );
            // double maxTempLinq = weeklyReport.Max(r => r.DegreesC); // Простіший спосіб
            Console.WriteLine($"\nНайвища t° (LINQ Aggregate): {maxTemp}°C");

            
            // --- Критерій: Комбіновані делегати (Бонус +1 бал) ---
            Console.WriteLine("\n--- 5. Бонус: Комбінований делегат (Multicast) ---");
            
            // Створюємо екземпляр делегата
            WeatherAlertHandler alertSystem = null;

            // Додаємо перший метод (сповіщувач)
            alertSystem += LogToConsole; 
            
            // Додаємо другий метод (сповіщувач)
            alertSystem += SendSmsAlert;

            Console.WriteLine("Тестуємо комбінований делегат (спрацюють 2 методи):");
            // Викликаємо делегат. Обидва методи (LogToConsole і SendSmsAlert) будуть викликані.
            alertSystem?.Invoke("Увага! Завтра очікується аномальна спека +31°C!");

            // Видалення методу з комбінації
            alertSystem -= LogToConsole;
            
            Console.WriteLine("\nТестуємо після видалення LogToConsole (спрацює 1 метод):");
            alertSystem?.Invoke("Сповіщення про можливий дощ.");
        }
    }
}