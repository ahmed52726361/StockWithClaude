using Inventory.Domain.Products;

namespace Inventory.Domain.Repository_Interfaces
{
    public interface IProductRepository : IRepository<Product> 
    {
        Task <Product?> FindByNameAsync(string name);
        Task<IReadOnlyList<Product>> FindLowStockAsync(int threshold);
        Task<bool> UpdateAsync(Product product);

    }
}
