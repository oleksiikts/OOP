using System;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace lab7v8
{
    // Generic helper class to handle retries with exponential backoff
    public static class RetryHelper
    {
        public static T ExecuteWithRetry<T>(
            Func<T> operation, 
            int retryCount = 3, 
            TimeSpan initialDelay = default, 
            Func<Exception, bool> shouldRetry = null)
        {
            int attempt = 0;
            
            while (true)
            {
                try
                {
                    return operation();
                }
                catch (Exception ex)
                {
                    // Check if we reached max retries or if the exception type is not whitelisted
                    if (attempt >= retryCount || (shouldRetry != null && !shouldRetry(ex)))
                    {
                        Console.WriteLine($"[Error] Final failure: {ex.Message}");
                        throw;
                    }

                    // Calculate exponential backoff delay
                    var delay = initialDelay * Math.Pow(2, attempt);
                    attempt++;

                    Console.WriteLine($"[Log] Attempt {attempt} failed ({ex.GetType().Name}). Retrying in {delay.TotalSeconds}s...");
                    Thread.Sleep(delay);
                }
            }
        }
    }

    // Class simulating file operations with programmed failures
    public class FileProcessor
    {
        private int _attemptCounter = 0;

        public void SaveUserData(string path, string userData)
        {
            _attemptCounter++;
            Console.WriteLine($"[FileProcessor] Trying to save to '{path}' (Call #{_attemptCounter})...");

            // Simulate IOException for the first 4 attempts
            if (_attemptCounter <= 4)
            {
                throw new IOException("Simulated disk error.");
            }

            Console.WriteLine("[FileProcessor] Data saved successfully.");
        }
    }

    // Class simulating network operations with programmed failures
    public class NetworkClient
    {
        private int _attemptCounter = 0;

        public bool PostUserData(string url, string userData)
        {
            _attemptCounter++;
            Console.WriteLine($"[NetworkClient] Sending POST to '{url}' (Call #{_attemptCounter})...");

            // Simulate HttpRequestException for the first 2 attempts
            if (_attemptCounter <= 2)
            {
                throw new HttpRequestException("503 Service Unavailable.");
            }

            Console.WriteLine("[NetworkClient] Data posted successfully.");
            return true;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var fileProcessor = new FileProcessor();
            var netClient = new NetworkClient();

            // Define logic: retry only for IOException or HttpRequestException
            Func<Exception, bool>? myRetryLogic = (ex) => ex is IOException || ex is HttpRequestException;

            Console.WriteLine("=== Scenario 1: File Processor (Fails 4 times, needs 5 attempts) ===");
            try
            {
                // We wrap the void method in a Func returning bool to satisfy the generic signature
                RetryHelper.ExecuteWithRetry<bool>(
                    () => 
                    {
                        fileProcessor.SaveUserData("data.txt", "User: Alex");
                        return true;
                    },
                    retryCount: 5, // Need enough retries to pass the 4 failures
                    initialDelay: TimeSpan.FromSeconds(0.5),
                    shouldRetry: myRetryLogic
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Process failed: {ex.Message}");
            }

            Console.WriteLine("\n=== Scenario 2: Network Client (Fails 2 times, needs 3 attempts) ===");
            try
            {
                bool result = RetryHelper.ExecuteWithRetry(
                    () => netClient.PostUserData("http://api.server.com", "User: Alex"),
                    retryCount: 3,
                    initialDelay: TimeSpan.FromSeconds(0.5),
                    shouldRetry: myRetryLogic
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Process failed: {ex.Message}");
            }
        }
    }
}