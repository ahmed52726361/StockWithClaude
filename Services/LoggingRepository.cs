using System;
using System.Collections.Generic;

namespace Inventory.Services
{
    internal class LoggingRepository<T> : IRepository<T> where T : IHasId
    {
        private List<T> _items = new List<T>();
        private Dictionary<int, T> _itemsById = new Dictionary<int, T>();

        public void Add(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            _items.Add(item);
            _itemsById.Add(item.Id, item);
            Console.WriteLine($"[LOG] Added item with Id {item.Id}");
        }

        public T Find(Predicate<T> predicate)
        {
            foreach (T item in _items)
            {
                if (predicate(item))
                {
                    Console.WriteLine($"[LOG] Find matched item with Id {item.Id}");
                    return item;
                }
            }
            Console.WriteLine("[LOG] Find found no match");
            return default(T);
        }

        public List<T> FindAll(Predicate<T> predicate)
        {
            List<T> items = new List<T>();
            foreach (T item in _items)
            {
                if (predicate(item))
                {
                    items.Add(item);
                }
            }
            Console.WriteLine($"[LOG] FindAll matched {items.Count} item(s)");
            return items;
        }

        public T GetById(int id)
        {
            if (_itemsById.TryGetValue(id, out T value))
            {
                Console.WriteLine($"[LOG] GetById({id}) found {value.Id}");
                return value;
            }
            Console.WriteLine($"[LOG] GetById({id}) found nothing");
            return default(T);
        }
    }
}