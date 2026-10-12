
using System;
using System.Collections.Generic;

namespace Inventory.Services
{
    internal class InMemoryRepository<T> : IInMemoryRepository<T>
        where T : class, IHasId
    {
        private readonly List<T> _items = new();
        private readonly Dictionary<int, T> _itemsById = new();

        public void Add(T item)
        {
            ArgumentNullException.ThrowIfNull(item);

            if (item.Id <= 0)
            {
                throw new InvalidOperationException(
                    "The item must have an ID before being added.");
            }

            if (_itemsById.ContainsKey(item.Id))
            {
                throw new InvalidOperationException(
                    $"An item with ID {item.Id} already exists.");
            }

            // Validate first, then update both collections.
            _itemsById.Add(item.Id, item);
            _items.Add(item);
        }

        public T? GetById(int id)
        {
            return _itemsById.TryGetValue(id, out T? item)
                ? item
                : null;
        }

        public T? Find(Predicate<T> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            return _items.Find(predicate);
        }

        public List<T> FindAll(Predicate<T> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            return _items.FindAll(predicate);
        }
    }
}
