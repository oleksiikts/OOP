# Принципи ISP (Interface Segregation) та DIP (Dependency Inversion)

## 1. Interface Segregation Principle (ISP)

**Суть принципу:** Клієнти не повинні залежати від методів, які вони не використовують. Краще створювати багато вузькоспеціалізованих інтерфейсів, ніж один загальний "жирний" інтерфейс.

### Приклад порушення ISP
Уявимо інтерфейс для багатофункціонального пристрою. Звичайний принтер змушений реалізовувати методи сканування та факсу, кидаючи винятки, бо він цього не вміє.

```csharp
// Погано: "Жирний" інтерфейс
public interface IMachine
{
    void Print(Document d);
    void Scan(Document d);
    void Fax(Document d);
}

// Принтер змушений реалізовувати методи, які йому не потрібні
public class SimplePrinter : IMachine
{
    public void Print(Document d) { /* Друк */ }
    
    public void Scan(Document d) 
    { 
        throw new NotImplementedException(); // Порушення!
    }
    
    public void Fax(Document d) 
    { 
        throw new NotImplementedException(); // Порушення!
    }
}

```

### Вирішення (Рефакторинг)

Розбиваємо великий інтерфейс на менші, сфокусовані частини.

```csharp
// Добре: Вузькі інтерфейси
public interface IPrinter { void Print(Document d); }
public interface IScanner { void Scan(Document d); }
public interface IFax { void Fax(Document d); }

// Тепер принтер реалізує тільки те, що вміє
public class SimplePrinter : IPrinter
{
    public void Print(Document d) { /* Друк */ }
}

// А "комбайн" може реалізувати все
public class XeroxMachine : IPrinter, IScanner, IFax 
{
    public void Print(Document d) { ... }
    public void Scan(Document d) { ... }
    public void Fax(Document d) { ... }
}

```

---

## 2. Dependency Inversion Principle (DIP)

**Суть принципу:** Модулі високого рівня не повинні залежати від модулів низького рівня. Обидва повинні залежати від абстракцій. Абстракції не повинні залежати від деталей.

### Переваги DIP (через Dependency Injection)

1. **Слабкий зв'язок (Loose Coupling):** Клас `NotificationService` не знає про конкретний `GmailSender`. Він працює з абстракцією `IMessageSender`. Це дозволяє легко замінити Gmail на Outlook або Telegram без зміни коду сервісу.
2. **Гнучкість конфігурації:** Залежності можна підміняти на етапі запуску програми (у `Program.cs` або DI-контейнері).

### Приклад коду

```csharp
// Абстракція (Інтерфейс)
public interface IMessageSender
{
    void Send(string message);
}

// Низькорівневий модуль (Деталь)
public class EmailSender : IMessageSender
{
    public void Send(string message) => Console.WriteLine($"Sending Email: {message}");
}

// Високорівневий модуль
public class NotificationService
{
    private readonly IMessageSender _sender;

    // Dependency Injection через конструктор
    public NotificationService(IMessageSender sender)
    {
        _sender = sender;
    }

    public void Notify(string msg)
    {
        _sender.Send(msg);
    }
}

```

---

## 3. Як ISP сприяє кращому DI та тестуванню

"Вузькі" інтерфейси (результат ISP) є фундаментом для якісного Dependency Injection та Unit-тестування:

1. **Чистіші Mock-об'єкти:** При тестуванні класу, який залежить від `IPrinter`, нам потрібно створити заглушку (mock) лише для одного методу `Print`. Якби ми залежали від "жирного" `IMachine`, нам довелося б писати заглушки для `Scan` і `Fax`, навіть якщо вони не використовуються в тесті.
2. **Зрозуміліші залежності:** Конструктор класу чітко каже, що йому потрібно. `new Editor(IPrinter printer)` — це зрозуміло. `new Editor(IMachine machine)` — вводить в оману, бо неясно, чи буде редактор сканувати, чи тільки друкувати.
3. **Легша заміна:** Чим менший інтерфейс, тим легше написати для нього нову реалізацію або адаптер (Decorator/Adapter pattern), не ламаючи систему.
