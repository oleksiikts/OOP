# God Object та Single Responsibility Principle (SRP)

## 1. Характеристики анти-патерну "God Object"

**God Object** (Божественний об'єкт) — це анти-патерн об'єктно-орієнтованого програмування, що описує об'єкт, який "знає занадто багато" або "робить занадто багато".

**Основні характеристики:**
* **Надмірна функціональність:** Клас містить логіку, що належить до різних доменних областей (бізнес-логіка, доступ до БД, логування, UI тощо).
* **Величезний розмір:** Часто містить тисячі рядків коду та велику кількість методів/властивостей.
* **Тісна зв'язність (Tight Coupling):** Зміна в одній частині системи часто вимагає змін у цьому об'єкті, і навпаки.
* **Складність тестування:** Через велику кількість залежностей та станів такий клас важко покрити unit-тестами.
* **Порушення SRP:** Він завжди порушує принцип єдиної відповідальності.

---

## 2. Приклад порушення SRP

Нижче наведено простий клас `UserManager`, який порушує принцип єдиної відповідальності.

```csharp
using System;
using System.IO;

public class UserManager
{
    public void RegisterUser(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Console.WriteLine("Invalid data");
            return;
        }

        File.AppendAllText("users.txt", $"{username}:{password}\n");

        Console.WriteLine($"Sending welcome email to {username}...");
    }
}