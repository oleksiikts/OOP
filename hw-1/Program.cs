using System;
using System.IO;

public class UserRepository
{
    public void Save(string username, string password)
    {
        File.AppendAllText("users.txt", $"{username}:{password}\n");
    }
}

public class EmailService
{
    public void SendWelcomeEmail(string username)
    {
        Console.WriteLine($"Sending welcome email to {username}...");
    }
}

public class UserService
{
    private readonly UserRepository _repository;
    private readonly EmailService _emailService;

    public UserService()
    {
        _repository = new UserRepository();
        _emailService = new EmailService();
    }

    public void Register(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Console.WriteLine("Invalid data");
            return;
        }

        _repository.Save(username, password);
        _emailService.SendWelcomeEmail(username);
    }
}