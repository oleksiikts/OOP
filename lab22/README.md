# Lab 22: Liskov Substitution Principle (LSP)

## 1. Аналіз порушення LSP
**Сценарій:** Ієрархія `User` (базовий) -> `GuestUser` (похідний).
**Проблема:** Базовий клас `BadUser` має метод `ChangePassword`, який передбачає успішну зміну пароля. Клас `BadGuestUser` успадковує цей метод, але викидає виняток `InvalidOperationException` при його виклику.

**Чому це порушення:**
Згідно з LSP, об'єкти підкласу повинні замінювати об'єкти базового класу, не ламаючи роботу програми. У нашому випадку:
1.  **Порушення контракту:** Базовий клас обіцяє "Я можу змінити пароль". Підклас каже "Я не можу".
2.  **Несподівана поведінка:** Клієнтський код (цикл `foreach` у Main), який працює з типом `BadUser`, не очікує помилки, але отримує її, коли потрапляє на гостя.

## 2. Альтернативне рішення (Refactoring)
**Підхід:** Розділення інтерфейсів (Interface Segregation) та зміна ієрархії.

Ми винесли здатність змінювати пароль в окремий інтерфейс `IPasswordManager`.
1.  `UserBase` — абстрактний клас, що містить лише спільні атрибути (Ім'я).
2.  `RegisteredUser` — наслідує `UserBase` ТА реалізує `IPasswordManager`.
3.  `GuestUser` — наслідує тільки `UserBase`.

**Результат:**
Тепер неможливо викликати метод `ChangePassword` у гостя на етапі компіляції. Компілятор не дозволить написати код, що призведе до помилки. Це повністю задовольняє принцип підстановки.

## 3. Результат роботи
```text
=== 1. Demonstration of LSP Violation ===
Trying to change password for Alice... [BadUser] Alice: Password changed to 'newPass'.
Trying to change password for Bob (Guest)... 
[ERROR] LSP Violation detected: Guests cannot change passwords!

=== 2. Demonstration of LSP Fix (Refactoring) ===
User Alice can change password... [RegisteredUser] Alice: Password successfully changed.
User Guest is a Guest and relies on readonly access.