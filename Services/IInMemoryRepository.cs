
using System;
using System.Collections.Generic;

namespace Inventory.Services
{
    internal interface IInMemoryRepository<T> where T : class, IHasId
    {
        void Add(T item);
        T? GetById(int id);
        T? Find(Predicate<T> predicate);
        List<T> FindAll(Predicate<T> predicate);
    }
}
