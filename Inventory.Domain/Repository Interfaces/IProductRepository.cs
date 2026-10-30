using Inventory.Products;

namespace Inventory.General_Interfaces.Repository_Interfaces
{
    public interface IProductRepository : IRepository<Product> 
    {
        Task <Product?> FindByNameAsync(string name);
        Task<IReadOnlyList<Product>> FindLowStockAsync(int threshold);
    }
}
