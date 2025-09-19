using System;
using System.Collections.Generic;

// Базовий клас Figure
public abstract class Figure
{
    // Абстрактний метод для обчислення площі
    public abstract double Area();
    
    // Абстрактний метод для обчислення об'єму (тільки для 3D фігур)
    public abstract double Volume();

    // Метод для виведення інформації про фігуру
    public virtual void DisplayInfo()
    {
        Console.WriteLine("This is a figure.");
    }
}

// Похідний клас Square
public class Square : Figure
{
    public double Side { get; set; }

    // Конструктор з викликом базового конструктора
    public Square(double side)
    {
        Side = side;
    }

    // Перевизначення методу для обчислення площі квадрата
    public override double Area()
    {
        return Side * Side;
    }

    // Перевизначення методу DisplayInfo
    public override void DisplayInfo()
    {
        Console.WriteLine($"Square with side {Side} has area {Area()}");
    }

    // Пустий об'єм для 2D фігури
    public override double Volume()
    {
        return 0;
    }
}

// Похідний клас Cube
public class Cube : Figure
{
    public double Side { get; set; }

    // Конструктор з викликом базового конструктора
    public Cube(double side)
    {
        Side = side;
    }

    // Перевизначення методу для обчислення площі (поверхня куба)
    public override double Area()
    {
        return 6 * (Side * Side); // Площа поверхні куба
    }

    // Перевизначення методу для обчислення об'єму куба
    public override double Volume()
    {
        return Side * Side * Side; // Об'єм куба
    }

    // Перевизначення методу DisplayInfo
    public override void DisplayInfo()
    {
        Console.WriteLine($"Cube with side {Side} has surface area {Area()} and volume {Volume()}");
    }
}

class Program
{
    static void Main()
    {
        // Створення об'єктів
        List<Figure> figures = new List<Figure>
        {
            new Square(5),
            new Cube(3),
            new Square(7),
            new Cube(4)
        };

        // Демонстрація поліморфізму
        foreach (var figure in figures)
        {
            figure.DisplayInfo(); // Поліморфізм в дії
        }
    }
}
