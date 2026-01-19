# Звіт з аналізу SOLID принципів (SRP, OCP) в Open-Source проєкті

## 1. Обраний проєкт

* **Назва:** nopCommerce
* **Опис:** Популярна платформа для електронної комерції на базі ASP.NET Core.
* **Посилання на GitHub:** [https://github.com/nopSolutions/nopCommerce](https://github.com/nopSolutions/nopCommerce)

---

## 2. Аналіз SRP (Single Responsibility Principle)

### 2.1. Приклади дотримання SRP

#### Клас: `EncryptionService`

* **Відповідальність:** Цей сервіс відповідає виключно за криптографічні операції (хешування паролів, шифрування тексту).
* **Обґрунтування:** Він не займається збереженням паролів у БД і не перевіряє права доступу. Він лише перетворює вхідні дані на зашифровані. Якщо зміниться алгоритм хешування (наприклад, з SHA256 на PBKDF2), ми змінимо тільки цей клас.

```csharp
public class EncryptionService : IEncryptionService
{
    // Тільки методи для шифрування/хешування
    public virtual string CreatePasswordHash(string password, string saltKey, string hashedPasswordFormat)
    {
        // Логіка хешування...
    }

    public virtual string EncryptText(string plainText, string privateKey = "")
    {
        // Логіка шифрування...
    }
}

```

#### Клас: `KeepAliveMiddleware`

* **Відповідальність:** Перехоплення HTTP-запиту виключно для перевірки доступності (ping) сторінки "KeepAlive", щоб запобігти засипанню пулу додатку.
* **Обґрунтування:** Цей клас робить одну вузьку річ у пайплайні запитів. Він не обробляє інші маршрути і не містить бізнес-логіки.

### 2.2. Приклади порушення SRP

#### Клас: `OrderProcessingService`

* **Множинні відповідальності:** Цей клас є класичним "God Object". Він займається:
1. Перевіркою наявності товару (Inventory).
2. Розрахунком податків та знижок.
3. Списанням коштів (взаємодія з платіжними шлюзами).
4. Відправкою email-сповіщень клієнту.
5. Генерацією PDF-інвойсів.
6. Збереженням даних у БД.


* **Проблеми:** Величезний розмір файлу (тисячі рядків). Зміна логіки відправки листів вимагає редагування класу обробки замовлень, що підвищує ризик зламати логіку розрахунку цін.

```csharp
public class OrderProcessingService : IOrderProcessingService
{
    // Метод робить ВСЕ підряд
    public virtual async Task<PlaceOrderResult> PlaceOrderAsync(ProcessPaymentRequest processPaymentRequest)
    {
        // 1. Валідація
        // 2. Блокування інвентарю
        // 3. Обробка платежу
        // 4. Збереження Order в БД
        // 5. Відправка Email (порушення SRP)
        // 6. Додавання Reward Points (порушення SRP)
    }
}

```

---

## 3. Аналіз OCP (Open/Closed Principle)

### 3.1. Приклади дотримання OCP

#### Модуль: Платіжні методи (`IPaymentMethod`)

* **Механізм розширення:** Використання інтерфейсу та патерну "Стратегія".
* **Обґрунтування:** Система дозволяє додавати нові способи оплати (PayPal, Stripe, LiqPay), не змінюючи код `OrderProcessingService` або `PaymentService`. Ми просто створюємо новий клас, що реалізує `IPaymentMethod`, і реєструємо його як плагін. Core-код залишається закритим для змін, але відкритим для розширення.

```csharp
// Інтерфейс (Контракт)
public interface IPaymentMethod : IPlugin
{
    Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest);
    // ... інші методи
}

// Нова реалізація (Розширення)
public class PayPalStandardPaymentProcessor : BasePlugin, IPaymentMethod
{
    public async Task<ProcessPaymentResult> ProcessPaymentAsync(...) 
    {
        // Логіка саме для PayPal
    }
}

```

#### Модуль: Розрахунок доставки (`IShippingRateComputationMethod`)

* **Механізм розширення:** Аналогічна система плагінів. Можна додати інтеграцію з "Нова Пошта" або "DHL", створивши новий клас-провайдер, не чіпаючи ядро системи.

### 3.2. Приклади порушення OCP

#### Сценарій: Експорт даних (гіпотетичний приклад на основі старих версій)

* **Проблема:** Використання `switch` або `if-else` для визначення формату експорту всередині менеджера, замість поліморфізму.
* **Наслідки:** Щоб додати новий формат (наприклад, JSON), розробнику доводиться відкривати існуючий перевірений клас `ExportManager` і дописувати туди новий `case`, ризикуючи зламати існуючий експорт у XML або Excel.

```csharp
// Поганий підхід (порушення OCP)
public void ExportOrders(List<Order> orders, ExportFormat format)
{
    if (format == ExportFormat.Xlsx)
    {
        // Логіка Excel...
    }
    else if (format == ExportFormat.Xml)
    {
        // Логіка XML...
    }
    else if (format == ExportFormat.Csv)
    {
        // Логіка CSV...
    }
    // Щоб додати JSON, треба змінити цей файл!
}

```

---

## 4. Загальні висновки

У проєкті **nopCommerce** чітко простежується еволюція архітектури:

1. **OCP реалізовано блискуче** завдяки архітектурі плагінів. Це дозволяє розробникам по всьому світу створювати розширення (оплати, доставки, віджети) без модифікації ядра. Це ідеальний приклад Open/Closed Principle.
2. **SRP дотримується не всюди**. Сервісні класи ("Services") часто перетворюються на контейнери для всієї бізнес-логіки, пов'язаної з сутністю (наприклад, `OrderProcessingService` або `ProductService`). Вони часто беруть на себе зайві обов'язки (логування, сповіщення, мапінг), що робить їх складними для тестування та підтримки.

**Рекомендація:** Для покращення SRP варто було б розбити великі сервіси на менші, вузькоспеціалізовані класи (наприклад, винести логіку відправки листів про замовлення в `OrderNotificationService`, а валідацію — в `OrderValidator`).