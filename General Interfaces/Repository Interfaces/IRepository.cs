using Inventory.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.General_Interfaces.Repository_Interfaces
{
    internal interface IRepository<T> where T : class,IHasId
    {
        Task AddAsync(T item);
        Task<T?> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task <bool> DeleteAsync(int id);
    }
}
