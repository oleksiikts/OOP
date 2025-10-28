namespace hw1.Cache
{
    // Допоміжний клас для зберігання значення та метаданих (часу додавання)
    public class CacheItem<T> where T : class // Застосовуємо обмеження
    {
        public T Value { get; set; }
        public DateTime AddedAt { get; private set; }

        public CacheItem(T value)
        {
            Value = value;
            AddedAt = DateTime.UtcNow; // Фіксуємо час додавання
        }
    }
}