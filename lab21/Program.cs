using System;
using System.Collections.Generic;

namespace lab21
{

    public interface IPricingStrategy
    {
        decimal CalculateCost(decimal dataSizeGb, int userCount);
    }
    public class PersonalPlanStrategy : IPricingStrategy
    {
        public decimal CalculateCost(decimal dataSizeGb, int userCount)
        {
            return dataSizeGb * 0.10m; 
        }
    }

    public class BusinessPlanStrategy : IPricingStrategy
    {
        public decimal CalculateCost(decimal dataSizeGb, int userCount)
        {

            return (dataSizeGb * 0.20m) + (userCount * 5.00m);
        }
    }

    public class EnterprisePlanStrategy : IPricingStrategy
    {
        public decimal CalculateCost(decimal dataSizeGb, int userCount)
        {

            decimal baseCost = (dataSizeGb * 0.15m) + (userCount * 10.00m);
            return baseCost * 1.20m; // +20%
        }
    }

    public class StudentPlanStrategy : IPricingStrategy
    {
        public decimal CalculateCost(decimal dataSizeGb, int userCount)
        {
    
            return dataSizeGb * 0.05m;
        }
    }


    public static class PricingStrategyFactory
    {
        public static IPricingStrategy CreateStrategy(string planType)
        {
            return planType.ToLower() switch
            {
                "personal" => new PersonalPlanStrategy(),
                "business" => new BusinessPlanStrategy(),
                "enterprise" => new EnterprisePlanStrategy(),
                "student" => new StudentPlanStrategy(), // Додали нову опцію
                _ => throw new ArgumentException("Unknown plan type")
            };
        }
    }

    public class CloudStorageService
    {
        public decimal CalculateTotal(decimal dataSizeGb, int userCount, IPricingStrategy strategy)
        {
            if (dataSizeGb < 0 || userCount < 0)
            {
                throw new ArgumentException("Data and users must be positive.");
            }

            return strategy.CalculateCost(dataSizeGb, userCount);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            CloudStorageService service = new CloudStorageService();

            while (true)
            {
                try
                {
                    Console.WriteLine("\n--- Cloud Storage Cost Calculator ---");
                    Console.WriteLine("Available plans: Personal, Business, Enterprise, Student");
                    Console.Write("Enter plan type (or 'exit'): ");
                    string? planInput = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(planInput) || planInput.ToLower() == "exit")
                        break;

                    IPricingStrategy strategy = PricingStrategyFactory.CreateStrategy(planInput);

                    Console.Write("Enter data volume (GB): ");
                    decimal data = decimal.Parse(Console.ReadLine() ?? "0");

                    Console.Write("Enter number of users: ");
                    int users = int.Parse(Console.ReadLine() ?? "0");

                    decimal cost = service.CalculateTotal(data, users, strategy);

                    Console.WriteLine($"\n>> Total Monthly Cost for {planInput}: ${cost:F2}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }
}