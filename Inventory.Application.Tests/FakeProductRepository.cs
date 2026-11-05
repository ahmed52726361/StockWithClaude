using Inventory.General_Interfaces.Repository_Interfaces;
using Inventory.Products;
using Microsoft.VisualBasic;
using System.Linq;

namespace Inventory.Application.Tests
{
    internal class FakeProductRepository : IProductRepository
    {
        private readonly List<Product> _products = new();
        private int _nextId=1;
        private static Product Copy(Product source)
        {
            Product copy = new(source.Name, source.Price, source.StockQty);
            copy.SetId(source.Id);
            return copy;
        }
        public Task<int> AddAsync(Product item)
        {
            int id = _nextId++;
            item.SetId(id);
            _products.Add(Copy(item));
            return Task.FromResult(id);
        }

        public  Task<Product?> FindByNameAsync(string name)
        {
            Product? stored=_products.FirstOrDefault(x =>string.Equals(x.Name,name,StringComparison.OrdinalIgnoreCase));
            return  Task.FromResult<Product?>(stored is null ? null : Copy(stored));
        }

        public  Task<Product?> GetByIdAsync(int id)
        {
            Product? stored=_products.FirstOrDefault(x=>x.Id == id);
            return  Task.FromResult<Product?>(stored is null ? null : Copy(stored));
        }

        public  Task<bool> UpdateAsync(Product product)
        {
            int index=_products.FindIndex(x=>x.Id==product.Id);
            if (index==-1)
            {
                return  Task.FromResult(false);
            }
            _products[index]=Copy(product);
            return Task.FromResult(true);
            
        }
        public Task<bool> DeleteAsync(int id)
        {
            int index = _products.FindIndex(x => x.Id == id);
            if (index==-1)
            {
                return Task.FromResult(false);
            }
            _products.RemoveAt(index);
            return Task.FromResult(true);
        }
        public Task<IReadOnlyList<Product>> FindLowStockAsync(int threshold)
        {
            IReadOnlyList<Product> result=_products
                .Where(P=>P.StockQty<=threshold)
                .Select(Copy)
                .ToList();
            return Task.FromResult(result);

        }
        public Task<IReadOnlyList<Product>> GetAllAsync()
        {
            IReadOnlyList<Product> result=_products.Select(Copy).ToList();
            return Task.FromResult(result);
        }
    }
}
