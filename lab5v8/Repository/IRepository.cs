using System;
using System.Collections.Generic;

namespace lab5v8.Repository
{
    // Узагальнений компонент (Generic)
    public interface IRepository<T>
    {
        void Add(T entity);
        void Remove(T entity);
        T Find(Func<T, bool> predicate);
        IEnumerable<T> All();
        IEnumerable<T> Where(Func<T, bool> predicate);
    }
}