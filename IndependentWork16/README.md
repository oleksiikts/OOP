# Independent Work 16: SRP & Decomposition of TaskScheduler

Цей проект демонструє рефакторинг монолітного класу планувальника завдань на окремі компоненти згідно з принципом єдиної відповідальності (SRP).

## UML Діаграма класів

```mermaid
classDiagram
    class SchedulerService {
        -ITaskValidator _validator
        -ITaskExecutor _executor
        -ILogger _logger
        -INotificationService _notifier
        +ProcessTask(TaskItem task)
    }

    class ITaskValidator {
        <<interface>>
        +IsValid(TaskItem task) bool
    }

    class ITaskExecutor {
        <<interface>>
        +Execute(TaskItem task)
    }

    class ILogger {
        <<interface>>
        +Log(string message)
    }

    class INotificationService {
        <<interface>>
        +SendNotification(string message)
    }

    class SimpleTaskValidator {
        +IsValid(TaskItem task) bool
    }

    class ConsoleTaskExecutor {
        +Execute(TaskItem task)
    }

    class ConsoleLogger {
        +Log(string message)
    }

    class EmailNotificationService {
        +SendNotification(string message)
    }

    SchedulerService ..> ITaskValidator
    SchedulerService ..> ITaskExecutor
    SchedulerService ..> ILogger
    SchedulerService ..> INotificationService

    ITaskValidator <|.. SimpleTaskValidator
    ITaskExecutor <|.. ConsoleTaskExecutor
    ILogger <|.. ConsoleLogger
    INotificationService <|.. EmailNotificationService
    [Logger] 15:30:01: Received task #1



### Приклад роботи 

[Executor] Running task: Database Backup...
[Logger] 15:30:01: Task #1 completed.
[Notification] Email sent: Task 'Database Backup' finished successfully.

[Logger] 15:30:01: Received task #2
[Logger] 15:30:01: Task #2 is invalid.


Відповіді на контрольні питання
1. Поясніть принцип єдиної відповідальності (SRP). Чому він важливий для проектування? SRP (Single Responsibility Principle) стверджує, що клас повинен мати лише одну причину для зміни. Це означає, що клас має виконувати лише одну логічну функцію. Це важливо, оскільки спрощує розуміння коду, полегшує тестування (unit-тести) та зменшує ризик того, що зміна в одній частині системи зламає іншу (знижує зв'язаність).

2. Що таке “God Object” і як він порушує SRP? "God Object" (Божественний об'єкт) — це анти-патерн, коли один клас бере на себе занадто багато відповідальностей (наприклад, валідація, логування, робота з БД, бізнес-логіка). Він порушує SRP, бо має безліч причин для змін і стає дуже складним для підтримки.

3. Як декомпозиція допомагає дотримуватися SRP? Наведіть приклад. Декомпозиція розбиває великий клас на менші. Наприклад, замість одного класу TaskScheduler, який робить все, ми виділяємо TaskValidator (тільки перевірка), TaskExecutor (тільки виконання) і Logger (тільки запис подій).

4. Як діаграми класів (UML) допомагають візуалізувати та аналізувати розподіл відповідальностей? UML діаграми наочно показують структуру класів та їхні зв'язки. Дивлячись на діаграму, можна легко побачити, від яких інтерфейсів залежить головний сервіс (через стрілки залежностей) і чи не перевантажений певний клас методами, що належать до різних доменів.

