using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory.Products;

namespace Inventory.General_Interfaces.Repository_Interfaces
{
    internal interface IProductRepository : IRepository<Product> 
    {
        Task<Product?> FindByNameAsync(string name);
        Task<IReadOnlyList<Product>> FindLowStockAsync(int threshold);
    }
}
