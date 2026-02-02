using System;
using System.Collections.Generic;

namespace lab22
{

    public class BadUser
    {
        public string Name { get; set; }
        public string Password { get; set; } = "1234";

        public virtual void ChangePassword(string newPassword)
        {
            Password = newPassword;
            Console.WriteLine($"[BadUser] {Name}: Password changed to '{newPassword}'.");
        }
    }

    public class BadGuestUser : BadUser
    {
        public override void ChangePassword(string newPassword)
        {
    
            throw new InvalidOperationException("Guests cannot change passwords!");
        }
    }

    public abstract class UserBase
    {
        public string Name { get; set; }
        
        public UserBase(string name) => Name = name;
    }

    public interface IPasswordManager
    {
        void ChangePassword(string newPassword);
    }

    public class RegisteredUser : UserBase, IPasswordManager
    {
        public string Password { get; private set; } = "default";

        public RegisteredUser(string name) : base(name) { }

        public void ChangePassword(string newPassword)
        {
            Password = newPassword;
            Console.WriteLine($"[RegisteredUser] {Name}: Password successfully changed.");
        }
    }

    public class GuestUser : UserBase
    {
        public GuestUser() : base("Guest") { }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== 1. Demonstration of LSP Violation ===");
            
            BadUser user1 = new BadUser { Name = "Alice" };
            BadUser guest1 = new BadGuestUser { Name = "Bob (Guest)" };

            List<BadUser> badUsers = new List<BadUser> { user1, guest1 };

            foreach (var u in badUsers)
            {
                try
                {
                    Console.Write($"Trying to change password for {u.Name}... ");
                    u.ChangePassword("newPass");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n[ERROR] LSP Violation detected: {ex.Message}");
                }
            }

            Console.WriteLine("\n=== 2. Demonstration of LSP Fix (Refactoring) ===");

            var regUser = new RegisteredUser("Alice");
            var gstUser = new GuestUser();

            List<UserBase> newUsers = new List<UserBase> { regUser, gstUser };

            foreach (var u in newUsers)
            {
                if (u is IPasswordManager pm)
                {
                    Console.Write($"User {u.Name} can change password... ");
                    pm.ChangePassword("securePass123");
                }
                else
                {
                    Console.WriteLine($"User {u.Name} is a Guest and relies on readonly access.");
                }
            }

            Console.ReadKey();
        }
    }
}