using System;
using System.Collections.Generic;
using System.Linq;

namespace lab5v8.Repository
{
    public class Repository<T> : IRepository<T>
    {
        // Приватне сховище в пам'яті
        private readonly List<T> _items = new List<T>();

        public void Add(T entity)
        {
            _items.Add(entity);
        }

        public IEnumerable<T> All()
        {
            return _items;
        }

        public T Find(Func<T, bool> predicate)
        {
            // FirstOrDefault безпечніший, ніж First, бо поверне null,
            // якщо нічого не знайдено, замість падіння з помилкою.
            return _items.FirstOrDefault(predicate);
        }

        public void Remove(T entity)
        {
            _items.Remove(entity);
        }

        public IEnumerable<T> Where(Func<T, bool> predicate)
        {
            return _items.Where(predicate);
        }
    }
}