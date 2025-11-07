// File: TemperatureRecord.cs

namespace Lab6_Delegates
{
    /// <summary>
    /// Клас для зберігання температурного запису (Варіант 7).
    /// </summary>
    public class TemperatureRecord
    {
        public string Day { get; set; }
        public double DegreesC { get; set; }

        public TemperatureRecord(string day, double degreesC)
        {
            Day = day;
            DegreesC = degreesC;
        }

        public override string ToString()
        {
            return $"{Day}: {DegreesC}°C";
        }
    }
}