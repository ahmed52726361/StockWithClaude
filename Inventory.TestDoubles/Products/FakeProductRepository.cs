using Inventory.Domain.Products;
using Inventory.Domain.Products.Exceptions;
using Inventory.Domain.Repository_Interfaces;

namespace Inventory.TestDoubles.Products;

public class FakeProductRepository : IProductRepository
{
    private readonly List<Product> _products = new();
    private int _nextId = 1;
    public bool ForceStockReductionToFail { get; set; }
    public bool SkipDuplicateNameCheck { get; set; }

    private static Product Copy(Product source)
    {
        Product copy = new(source.Name, source.Price, source.StockQty);
        copy.SetId(source.Id);
        return copy;
    }
    private  Product ProductExists(string name)
    {
        Product? result = _products.FirstOrDefault(p =>
    string.Equals(
        p.Name,
        name,
        StringComparison.OrdinalIgnoreCase));

        return result is null ? null : Copy(result);
    }
    public Task<int> AddAsync(Product item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (SkipDuplicateNameCheck==false)
        {
            if (ProductExists(item.Name)!=null)
                throw new DuplicateProductNameException(item.Name);
            
        }

        int id = _nextId++;

        item.SetId(id);
        _products.Add(Copy(item));
        SkipDuplicateNameCheck = false;

        return Task.FromResult(id);
    }
    public Task<Product?> FindByNameAsync(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        Product? product = ProductExists(name);
        return Task.FromResult(product);
    }
    public Task<Product?> GetByIdAsync(int id)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(id, 0);

        Product? product = _products.FirstOrDefault(p => p.Id == id);

        return Task.FromResult(
            product is null ? null : Copy(product));
    }

    public Task<bool> UpdateAsync(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        int index = _products.FindIndex(p => p.Id == product.Id);

        if (index == -1)
            return Task.FromResult(false);

        _products[index] = Copy(product);

        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(int id)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(id, 0);

        int index = _products.FindIndex(p => p.Id == id);

        if (index == -1)
            return Task.FromResult(false);

        _products.RemoveAt(index);

        return Task.FromResult(true);
    }

    public Task<IReadOnlyList<Product>> GetAllAsync()
    {
        IReadOnlyList<Product> result = _products
            .Select(Copy)
            .ToList();

        return Task.FromResult(result);
    }

    public Task<IReadOnlyList<Product>> FindLowStockAsync(int threshold)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(threshold, 0);

        IReadOnlyList<Product> result = _products
            .Where(p => p.StockQty <= threshold)
            .Select(Copy)
            .ToList();

        return Task.FromResult(result);
    }

    public Task<bool> TryReduceStockAsync(int productId, int quantity)
    {
        if (ForceStockReductionToFail)
            return Task.FromResult(false);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(productId, 0);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(quantity, 0);

        int index = _products.FindIndex(p => p.Id == productId);

        if (index == -1 || _products[index].StockQty < quantity)
            return Task.FromResult(false);

        Product updated = Copy(_products[index]);
        updated.Sale(quantity);

        _products[index] = updated;

        return Task.FromResult(true);
    }
}
