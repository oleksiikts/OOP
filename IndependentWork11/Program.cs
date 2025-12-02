using Polly;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace IndependentWork11;

class Program
{
    // Імітація зовнішніх сервісів
    private static int _apiFailureCount = 0;
    private static int _dbFailureCount = 0;

    static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== Polly Resilience Demo ===\n");

        await Scenario1_ApiRetryAsync();
        Console.WriteLine(new string('-', 40));
        
        await Scenario2_DatabaseCircuitBreakerAsync();
        Console.WriteLine(new string('-', 40));

        await Scenario3_TimeoutFallbackAsync();

        Console.WriteLine("\nDemo completed.");
    }

    // --- СЦЕНАРІЙ 1: Нестабільний API (Retry + Jitter) ---
    static async Task Scenario1_ApiRetryAsync()
    {
        Log("SCENARIO 1: External API (Retry w/ Exponential Backoff)", ConsoleColor.Cyan);

        // Політика: Повторювати 3 рази.
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (exception, timeSpan, attempt, context) =>
                {
                    Log($"[Polly] Помилка: {exception.Message}. Спроба #{attempt}. Чекаємо {timeSpan.TotalSeconds}с...", ConsoleColor.Yellow);
                });

        try
        {
            await retryPolicy.ExecuteAsync(async () =>
            {
                await MockExternalApiCallAsync();
            });
        }
        catch (Exception ex)
        {
            Log($"[FATAL] Всі спроби вичерпано. Лог помилки: {ex.Message}", ConsoleColor.Red);
        }
    }

    // --- СЦЕНАРІЙ 2: Перевантажена БД (Circuit Breaker) ---
    static async Task Scenario2_DatabaseCircuitBreakerAsync()
    {
        Log("\nSCENARIO 2: Database Overload (Circuit Breaker)", ConsoleColor.Cyan);

        // Політика: Circuit Breaker
        var circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 2,
                durationOfBreak: TimeSpan.FromSeconds(5),
                onBreak: (ex, breakDelay) =>
                {
                    Log($"[Polly] Circuit Breaker ВІДКРИТО на {breakDelay.TotalSeconds}с через: {ex.Message}", ConsoleColor.Magenta);
                },
                onReset: () => Log("[Polly] Circuit Breaker ЗАКРИТО. Система відновилась.", ConsoleColor.Green),
                onHalfOpen: () => Log("[Polly] Circuit Breaker НАПІВ-ВІДКРИТИЙ. Перевірка доступності...", ConsoleColor.DarkYellow)
            );

        // Імітуємо цикл запитів
        for (int i = 0; i < 6; i++)
        {
            try
            {
                Log($"Запит #{i + 1} до БД...");
                await circuitBreakerPolicy.ExecuteAsync(MockDatabaseCallAsync);
                Log("Успіх!", ConsoleColor.Green);
            }
            catch (BrokenCircuitException)
            {
                Log("Запит відхилено (Circuit Breaker Open).", ConsoleColor.DarkRed);
            }
            catch (Exception ex)
            {
                Log($"Помилка виконання: {ex.Message}", ConsoleColor.Red);
            }

            await Task.Delay(1000); // Пауза між запитами користувача
        }
    }

    // --- СЦЕНАРІЙ 3: Повільна операція (Timeout + Fallback) ---
   // --- СЦЕНАРІЙ 3: Повільна операція (Timeout + Fallback) ---
static async Task Scenario3_TimeoutFallbackAsync()
{
    Log("\nSCENARIO 3: Slow Operation (Timeout + Fallback)", ConsoleColor.Cyan);

    // Timeout повертає string
    var timeoutPolicy = Policy
        .TimeoutAsync<string>(TimeSpan.FromSeconds(2), TimeoutStrategy.Pessimistic);

    // Fallback повертає string
    var fallbackPolicy = Policy<string>
        .Handle<TimeoutRejectedException>()
        .Or<Exception>()
        .FallbackAsync(
            fallbackValue: "cached_data_v1.json",
            onFallbackAsync: async (outcome) =>
            {
                string errorMsg = outcome.Exception?.Message ?? "Unknown error";
                Log($"[Polly] Операція надто довга ({errorMsg}). Використовуємо Fallback.", ConsoleColor.Yellow);
                await Task.CompletedTask;
            });

    // Об’єднуємо політики
    var strategy = Policy.WrapAsync<string>(fallbackPolicy, timeoutPolicy);

    var result = await strategy.ExecuteAsync(async () =>
    {
        return await MockHeavyCalculationAsync(); 
    });

    Log($"Фінальний результат для клієнта: {result}", ConsoleColor.White);
}


    // --- MOCK SERVICES (ІМІТАЦІЯ) ---

    static async Task MockExternalApiCallAsync()
    {
        _apiFailureCount++;
        if (_apiFailureCount <= 2)
        {
            throw new HttpRequestException("503 Service Unavailable");
        }
        Console.WriteLine("   -> API повернув 200 OK");
        await Task.CompletedTask;
    }

    static async Task MockDatabaseCallAsync()
    {
        _dbFailureCount++;
        if (_dbFailureCount <= 4)
        {
            throw new Exception("Connection Timeout");
        }
        Console.WriteLine("   -> Дані отримано з БД");
        await Task.CompletedTask;
    }

    static async Task<string> MockHeavyCalculationAsync()
    {
        Console.WriteLine("   -> Початок важкої обробки...");
        await Task.Delay(5000); 
        return "fresh_data_v2.json";
    }

    // --- HELPER FOR LOGGING ---
    static void Log(string message, ConsoleColor color = ConsoleColor.Gray)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}