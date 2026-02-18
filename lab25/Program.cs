using System;
using System.IO;

namespace lab25
{
    public interface ILogger
    {
        void Log(string message);
    }

    public class ConsoleLogger : ILogger
    {
        public void Log(string message) => Console.WriteLine($"[Console Log]: {message}");
    }

    public class FileLogger : ILogger
    {
        private readonly string _path = "log.txt";
        public void Log(string message) 
        {
            File.AppendAllText(_path, $"[File Log]: {message}{Environment.NewLine}");
            Console.WriteLine("[System]: Message logged to file.");
        }
    }

    public abstract class LoggerFactory
    {
        public abstract ILogger CreateLogger();
    }

    public class ConsoleLoggerFactory : LoggerFactory
    {
        public override ILogger CreateLogger() => new ConsoleLogger();
    }

    public class FileLoggerFactory : LoggerFactory
    {
        public override ILogger CreateLogger() => new FileLogger();
    }

    public class LoggerManager
    {
        private static LoggerManager? _instance;
        private LoggerFactory _factory;

        private LoggerManager(LoggerFactory factory)
        {
            _factory = factory;
        }

        public static LoggerManager GetInstance(LoggerFactory factory)
        {
            if (_instance == null) _instance = new LoggerManager(factory);
            return _instance;
        }

        public void SetFactory(LoggerFactory factory) => _factory = factory;

        public void Log(string message) => _factory.CreateLogger().Log(message);
    }

    public interface IDataProcessorStrategy
    {
        string Process(string data);
    }

    public class EncryptDataStrategy : IDataProcessorStrategy
    {
        public string Process(string data) => $"Encrypted({data})";
    }

    public class CompressDataStrategy : IDataProcessorStrategy
    {
        public string Process(string data) => $"Compressed({data})";
    }

    public class DataContext
    {
        private IDataProcessorStrategy _strategy;

        public DataContext(IDataProcessorStrategy strategy) => _strategy = strategy;

        public void SetStrategy(IDataProcessorStrategy strategy) => _strategy = strategy;

        public string ExecuteStrategy(string data) => _strategy.Process(data);
    }

    public class DataPublisher
    {
        public event EventHandler<string>? DataProcessed;

        public void PublishDataProcessed(string info)
        {
            DataProcessed?.Invoke(this, info);
        }
    }

    public class ProcessingLoggerObserver
    {
        public void OnDataProcessed(object? sender, string info)
        {
            LoggerManager.GetInstance(new ConsoleLoggerFactory()).Log($"Observer received: {info}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== SCENARIO 1: FULL INTEGRATION ===");
            var loggerManager = LoggerManager.GetInstance(new ConsoleLoggerFactory());
            var dataContext = new DataContext(new EncryptDataStrategy());
            var publisher = new DataPublisher();
            var observer = new ProcessingLoggerObserver();

            publisher.DataProcessed += observer.OnDataProcessed;

            string rawData = "SecretMessage";
            string processedData = dataContext.ExecuteStrategy(rawData);
            publisher.PublishDataProcessed(processedData);

            Console.WriteLine("\n=== SCENARIO 2: DYNAMIC LOGGER CHANGE ===");
            loggerManager.SetFactory(new FileLoggerFactory());
            
            processedData = dataContext.ExecuteStrategy("NewData");
            publisher.PublishDataProcessed(processedData);

            Console.WriteLine("\n=== SCENARIO 3: DYNAMIC STRATEGY CHANGE ===");
            loggerManager.SetFactory(new ConsoleLoggerFactory()); 
            dataContext.SetStrategy(new CompressDataStrategy());

            processedData = dataContext.ExecuteStrategy("LargeFileContent");
            publisher.PublishDataProcessed(processedData);

            Console.WriteLine("\nDemo finished. Press any key...");
            Console.ReadKey();
        }
    }
}