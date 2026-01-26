using System;

namespace IndependentWork16
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public interface ITaskValidator
    {
        bool IsValid(TaskItem task);
    }

    public interface ITaskExecutor
    {
        void Execute(TaskItem task);
    }

    public interface ILogger
    {
        void Log(string message);
    }

    public interface INotificationService
    {
        void SendNotification(string message);
    }

    public class SimpleTaskValidator : ITaskValidator
    {
        public bool IsValid(TaskItem task)
        {
            return !string.IsNullOrWhiteSpace(task.Name);
        }
    }

    public class ConsoleTaskExecutor : ITaskExecutor
    {
        public void Execute(TaskItem task)
        {
            Console.WriteLine($"[Executor] Running task: {task.Name}...");
        }
    }

    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine($"[Logger] {DateTime.Now:HH:mm:ss}: {message}");
        }
    }

    public class EmailNotificationService : INotificationService
    {
        public void SendNotification(string message)
        {
            Console.WriteLine($"[Notification] Email sent: {message}");
        }
    }

    public class SchedulerService
    {
        private readonly ITaskValidator _validator;
        private readonly ITaskExecutor _executor;
        private readonly ILogger _logger;
        private readonly INotificationService _notifier;

        public SchedulerService(
            ITaskValidator validator,
            ITaskExecutor executor,
            ILogger logger,
            INotificationService notifier)
        {
            _validator = validator;
            _executor = executor;
            _logger = logger;
            _notifier = notifier;
        }

        public void ProcessTask(TaskItem task)
        {
            _logger.Log($"Received task #{task.Id}");

            if (!_validator.IsValid(task))
            {
                _logger.Log($"Task #{task.Id} is invalid.");
                return;
            }

            _executor.Execute(task);
            _logger.Log($"Task #{task.Id} completed.");
            _notifier.SendNotification($"Task '{task.Name}' finished successfully.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ITaskValidator validator = new SimpleTaskValidator();
            ITaskExecutor executor = new ConsoleTaskExecutor();
            ILogger logger = new ConsoleLogger();
            INotificationService notifier = new EmailNotificationService();

            SchedulerService scheduler = new SchedulerService(validator, executor, logger, notifier);

            TaskItem validTask = new TaskItem { Id = 1, Name = "Database Backup" };
            scheduler.ProcessTask(validTask);

            Console.WriteLine();

            TaskItem invalidTask = new TaskItem { Id = 2, Name = "" };
            scheduler.ProcessTask(invalidTask);

            Console.ReadKey();
        }
    }
}