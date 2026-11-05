using Inventory.Domain;

namespace Inventory.Domain.Repository_Interfaces
{
    public interface IRepository<T> where T : class,IHasId
    {
        Task<int> AddAsync(T item);
        Task<T?> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task <bool> DeleteAsync(int id);
    }
}
