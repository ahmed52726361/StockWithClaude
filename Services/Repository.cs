using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Services
{
    internal class Repository<T> : IRepository<T> where T : IHasId
    {
        private List<T> _items=new List<T>();
        private Dictionary<int, T> _itemsById = new Dictionary<int, T>();
        public void Add(T item)
        {
            if (item==null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            _items.Add(item);
            _itemsById.Add(item.Id, item);
        }
        public T Find(Predicate<T> predicate)
        {
            foreach (T item in _items)
            {
                if (predicate(item))
                {
                    return item;
                }
            }
            return default;
        }
        public List<T> FindAll(Predicate<T> predicate)
        {
            List<T> items = new List<T>();
            foreach(T item in _items)
            {
                if (predicate(item))
                {
                    items.Add(item);
                }
            }
            return items;
        }
        public T GetById(int id)
        {
            if (_itemsById.TryGetValue(id,out T value))
            {
                return value;
            }
            return default;
        }

    }
}
