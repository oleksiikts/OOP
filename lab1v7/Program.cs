using System;

class Phone
{
    private string _brand;
    private string _model;
    private int _batteryLevel;

    public Phone(string brand, string model, int batteryLevel)
    {
        _brand = brand;
        _model = model;
        _batteryLevel = batteryLevel;
    }

    public int BatteryLevel
    {
        get { return _batteryLevel; }
        set 
        {
            if (value >= 0 && value <= 100)
                _batteryLevel = value;
            else
                Console.WriteLine("Invalid battery level. It should be between 0 and 100.");
        }
    }

    public void Call(string number)
    {
        if (_batteryLevel > 0)
        {
            Console.WriteLine($"Calling {number} from {_brand} {_model}...");
            _batteryLevel -= 10;
            Console.WriteLine($"Battery level: {_batteryLevel}%");
        }
        else
        {
            Console.WriteLine("Battery is empty! Please charge your phone.");
        }
    }

    public void DisplayInfo()
    {
        Console.WriteLine($"Phone Brand: {_brand}");
        Console.WriteLine($"Phone Model: {_model}");
        Console.WriteLine($"Battery Level: {_batteryLevel}%");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Phone phone1 = new Phone("Apple", "iPhone 13", 80);
        Phone phone2 = new Phone("Samsung", "Galaxy S21", 50);

        phone1.DisplayInfo();
        phone2.DisplayInfo();

        phone1.Call("123-456-7890");
        phone2.Call("987-654-3210");

        phone1.DisplayInfo();
        phone2.DisplayInfo();
    }
}
