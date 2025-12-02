

# Звіт до самостійної роботи №11
**Тема:** Кейси Polly/Retry

**Студент:** [Купець Олексій]

## Вступ
Метою роботи було дослідження бібліотеки **Polly** для забезпечення відмовостійкості (resilience) .NET застосунків. Було реалізовано та проаналізовано три реальні сценарії обробки збоїв.

---

## Сценарій 1: Нестабільний зовнішній API

### 1. Опис проблеми
При інтеграції з мікросервісами або зовнішніми API часто виникають **транзієнтні помилки** (503 Service Unavailable, втрата пакетів). Якщо просто викинути виняток, користувач отримає помилку, хоча повторний запит через секунду міг би бути успішним.

### 2. Обрана політика: `WaitAndRetry` (Retry Pattern)
Використано стратегію **Retry з експоненційною затримкою**.
* **Обґрунтування:** Це дозволяє пережити короткочасні збої. Експоненційна затримка ($2^n$) запобігає перевантаженню сервісу, який і так працює нестабільно ("Backoff strategy").

### 3. Реалізація (C#)
```csharp
var retryPolicy = Policy
    .Handle<HttpRequestException>()
    .WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
        onRetry: (exception, timeSpan, attempt, context) =>
        {
            Log($"[Polly] Помилка: {exception.Message}. Спроба #{attempt}...");
        });
````

### 4\. Лог виконання

```text
SCENARIO 1: External API (Retry w/ Exponential Backoff)
[Polly] Помилка: 503 Service Unavailable. Спроба #1. Чекаємо 2с...
[Polly] Помилка: 503 Service Unavailable. Спроба #2. Чекаємо 4с...
   -> API повернув 200 OK
```

-----

## Сценарій 2: Перевантажена База Даних

### 1\. Опис проблеми

Коли база даних перевантажена, тисячі клієнтів, що намагаються "достукатися" до неї одночасно (Retry), створюють ефект DDoS-атаки, не даючи базі відновитися.

### 2\. Обрана політика: `Circuit Breaker` (Запобіжник)

Використано патерн **Circuit Breaker**.

  * **Обґрунтування:** Якщо кількість помилок перевищує поріг (2), запобіжник "розмикає ланцюг". Це дає системі час на охолодження. Всі запити в цей період миттєво відхиляються без звернення до реальної БД.

### 3\. Реалізація (C\#)

```csharp
var circuitBreakerPolicy = Policy
    .Handle<Exception>()
    .CircuitBreakerAsync(
        exceptionsAllowedBeforeBreaking: 2,
        durationOfBreak: TimeSpan.FromSeconds(5),
        onBreak: (ex, breakDelay) => Log($"[Polly] Circuit Breaker ВІДКРИТО..."),
        onReset: () => Log("[Polly] Circuit Breaker ЗАКРИТО."),
        onHalfOpen: () => Log("[Polly] Circuit Breaker НАПІВ-ВІДКРИТИЙ.")
    );
```

### 4\. Лог виконання

```text
SCENARIO 2: Database Overload (Circuit Breaker)
Запит #1 до БД...
Помилка виконання: Connection Timeout
Запит #2 до БД...
[Polly] Circuit Breaker ВІДКРИТО на 5с через: Connection Timeout
Помилка виконання: Connection Timeout
Запит #3 до БД...
Запит відхилено (Circuit Breaker Open).
...
Запит #6 до БД...
Запит відхилено (Circuit Breaker Open).
```

-----

## Сценарій 3: Довга операція (SLA Violation)

### 1\. Опис проблеми

Клієнт очікує відповідь за фіксований час (SLA, наприклад 2с). Якщо операція зависла, краще повернути користувачеві застарілі дані (кеш) або дефолтне значення, ніж змушувати його чекати вічно або показувати критичну помилку.

### 2\. Обрана політика: `Timeout` + `Fallback` (Policy Wrap)

Використано комбінацію політик:

  * **Timeout:** Жорстко перериває виконання через 2 секунди.
  * **Fallback:** Перехоплює помилку тайм-ауту і повертає резервні дані ("Graceful Degradation").

### 3\. Реалізація (C\#)

```csharp
var timeoutPolicy = Policy.TimeoutAsync(TimeSpan.FromSeconds(2));

var fallbackPolicy = Policy<string>
    .Handle<TimeoutRejectedException>()
    .FallbackAsync(fallbackValue: "cached_data_v1.json");

// Обгортаємо: Fallback ловить помилки від Timeout
var strategy = Policy.WrapAsync(fallbackPolicy, timeoutPolicy);
```

### 4\. Лог виконання

```text
SCENARIO 3: Slow Operation (Timeout + Fallback)
   -> Початок важкої обробки...
[Polly] Операція надто довга (The delegate executed asynchronously...). Використовуємо Fallback.
Фінальний результат для клієнта: cached_data_v1.json
```

-----

<p align="center"> <i>![Скріншот консолі](https://prnt.sc/L4jhVrBrjO3M)</i> </p>


## Висновки

Використання бібліотеки Polly дозволяє писати більш надійний та чистий код:

1.  **Розділення відповідальності:** Логіка обробки помилок винесена окремо від бізнес-логіки.
2.  **Гнучкість:** Політики легко комбінувати (`Policy.Wrap`).
3.  **User Experience:** Система не падає, а відновлюється (Retry) або коректно деградує (Fallback), що є критично важливим для Enterprise-рішень.



<p align="center"> <i>![Скріншот консолі](https://prnt.sc/L4jhVrBrjO3M)</i> </p>

<!-- end list -->

```
```