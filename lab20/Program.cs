using System;

namespace lab20
{
    public class Order
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; } 
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "New";
    }

    public interface IOrderValidator
    {
        bool IsValid(Order order);
    }

    public interface IOrderRepository
    {
        void Save(Order order);
        Order GetById(int id);
    }

    public interface IEmailService
    {
        void SendOrderConfirmation(Order order);
    }
    
    public class OrderValidator : IOrderValidator
    {
        public bool IsValid(Order order)
        {
            return order.TotalAmount > 0;
        }
    }

    public class InMemoryOrderRepository : IOrderRepository
    {
        public void Save(Order order)
        {
            Console.WriteLine($"[Database] Order #{order.Id} saved for {order.CustomerName}.");
        }

        public Order GetById(int id)
        {
            return new Order { Id = id, CustomerName = "Test", TotalAmount = 100 };
        }
    }

    public class ConsoleEmailService : IEmailService
    {
        public void SendOrderConfirmation(Order order)
        {
            Console.WriteLine($"[Email] Confirmation sent to {order.CustomerName} for Order #{order.Id}.");
        }
    }

    public class OrderService
    {
        private readonly IOrderValidator _validator;
        private readonly IOrderRepository _repository;
        private readonly IEmailService _emailService;

        public OrderService(IOrderValidator validator, IOrderRepository repository, IEmailService emailService)
        {
            _validator = validator;
            _repository = repository;
            _emailService = emailService;
        }

        public void ProcessOrder(Order order)
        {
            Console.WriteLine($"\n--- Processing Order #{order.Id} ---");

            if (!_validator.IsValid(order))
            {
                Console.WriteLine($"[Error] Order #{order.Id} is invalid (Amount must be > 0).");
                return;
            }

            order.Status = "Processed";
            _repository.Save(order);
            _emailService.SendOrderConfirmation(order);
            
            Console.WriteLine($"Order #{order.Id} completed successfully.");
        }
    }

    // --- Main ---
    class Program
    {
        static void Main(string[] args)
        {
            IOrderValidator validator = new OrderValidator();
            IOrderRepository repository = new InMemoryOrderRepository();
            IEmailService emailService = new ConsoleEmailService();

            OrderService orderService = new OrderService(validator, repository, emailService);

            Order validOrder = new Order 
            { 
                Id = 101, 
                CustomerName = "Oleksandr", 
                TotalAmount = 500.00m 
            };
            orderService.ProcessOrder(validOrder);

            Order invalidOrder = new Order 
            { 
                Id = 102, 
                CustomerName = "Incognito", 
                TotalAmount = -50.00m 
            };
            orderService.ProcessOrder(invalidOrder);

            Console.ReadKey();
        }
    }
}