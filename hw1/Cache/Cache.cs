using System.Collections.Generic;
using System.Linq;

namespace hw1.Cache
{
    // --- Узагальнений кеш з обмеженням "class" ---
    // Це означає, що 'T' має бути посилальним типом (наприклад, string, MyClass),
    // але не може бути int, double, bool.
    public class Cache<T> where T : class
    {
        // Використовуємо Dictionary для швидкого доступу по ключу
        private readonly Dictionary<string, CacheItem<T>> _cache = new Dictionary<string, CacheItem<T>>();
        
        // Обмеження на максимальний розмір кешу
        private readonly int _maxSize;

        public Cache(int maxSize)
        {
            _maxSize = maxSize > 0 ? maxSize : 10; // Базове обмеження
        }

        public void Add(string key, T value)
        {
            if (_cache.Count >= _maxSize)
            {
                // --- Алгоритм видалення старих елементів (FIFO) ---
                EvictOldest();
            }
            
            _cache[key] = new CacheItem<T>(value);
            Console.WriteLine($"[Cache] Додано: {key}");
        }

        public T? Get(string key)
        {
            if (_cache.TryGetValue(key, out CacheItem<T>? item))
            {
                return item.Value;
            }
            return null; // Повертаємо null, якщо нічого не знайдено
        }

        // --- Алгоритм видалення (Eviction) ---
        // Видаляє найстаріший елемент з кешу
        private void EvictOldest()
        {
            // Це проста, хоч і не найшвидша (O(n)) реалізація
            // для демонстрації алгоритму.
            if (_cache.Count == 0) return;

            string? keyToRemove = null;
            DateTime oldestTime = DateTime.UtcNow;

            foreach (var pair in _cache)
            {
                if (pair.Value.AddedAt < oldestTime)
                {
                    oldestTime = pair.Value.AddedAt;
                    keyToRemove = pair.Key;
                }
            }

            if (keyToRemove != null)
            {
                _cache.Remove(keyToRemove);
                Console.WriteLine($"[Cache Evict] Видалено старий елемент: {keyToRemove}");
            }
        }

        public void DisplayCacheContents()
        {
            Console.WriteLine("\n--- Вміст кешу ---");
            if (_cache.Count == 0)
            {
                Console.WriteLine("Кеш порожній.");
                return;
            }
            
            foreach (var pair in _cache)
            {
                Console.WriteLine($"  Key: {pair.Key}, Value: {pair.Value.Value}, Added: {pair.Value.AddedAt:HH:mm:ss.fff}");
            }
            Console.WriteLine("--------------------");
        }

        // --- Метод для отримання відсортованих даних ---
        public List<CacheItem<T>> GetSortedByDate()
        {
            // Отримуємо всі елементи з кешу
            var items = new List<CacheItem<T>>(_cache.Values);

            // Викликаємо наш власний алгоритм сортування
            // Передаємо функцію-компаратор
            Sorting.InsertionSort(items, (item1, item2) => item1.AddedAt.CompareTo(item2.AddedAt));

            return items;
        }
    }
}