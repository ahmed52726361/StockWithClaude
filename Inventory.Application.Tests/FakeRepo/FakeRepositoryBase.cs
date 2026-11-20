using Inventory.Domain;

namespace Inventory.Application.Tests.FakeRepo
{
    public abstract class FakeRepositoryBase<T> where T : class, IReceivesDatabaseId
    {

        private readonly List<T> _items = new();
        private int _nextId = 1;

        // Each entity knows how to copy itself. The base class can't.
        protected abstract T Copy(T source);

        // The stored items, for the specific Find methods of the derived fakes
        protected IReadOnlyList<T> Items => _items;
        protected virtual void OnAdded(T original) { }

        public Task<int> AddAsync(T item)
        {
            ArgumentNullException.ThrowIfNull(item);
            item.SetId(_nextId);
            _nextId++;
            _items.Add(Copy(item));
            OnAdded(item);
            return Task.FromResult(item.Id);
        }

        public Task<T?> GetByIdAsync(int id)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(id, 0);
            T? stored = _items.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(stored is null ? null : Copy(stored));
        }

        public Task<IReadOnlyList<T>> GetAllAsync()
            => Task.FromResult<IReadOnlyList<T>>(_items.Select(Copy).ToList());

        public Task<bool> DeleteAsync(int id)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(id, 0);
            int index = _items.FindIndex(x => x.Id == id);
            if (index == -1)
                return Task.FromResult(false);
            _items.RemoveAt(index);
            return Task.FromResult(true);
        }

        // Helper for the Find methods: filter, then return copies
        protected IReadOnlyList<T> CopiesWhere(Func<T, bool> predicate)
            => _items.Where(predicate).Select(Copy).ToList();
    }
}