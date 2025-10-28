using System;

namespace lab5v8.Models.Exceptions
{
   
    public class InvalidPackageException : Exception
    {
        public InvalidPackageException(string message) : base(message)
        {
        }
    }
}