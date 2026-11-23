
using Inventory.Domain.Products;
using Inventory.Domain.Products.Exceptions;
using Inventory.Domain.Repository_Interfaces;

namespace Inventory.RepositoryContractTests.Products;

public abstract class ProductRepositoryContract
{
    protected abstract IProductRepository CreateRepository();

    [Fact]
    public async Task AddAsync_DuplicateName_ThrowsDuplicateProductNameException()
    {
        IProductRepository repository = CreateRepository();

        string name = "Duplicate-Test-" + Guid.NewGuid();

        Product first = new(name, 10m, 5);
        Product second = new(name, 20m, 10);

        try
        {
            await repository.AddAsync(first);

            Assert.True(first.Id > 0);

            await Assert.ThrowsAsync<DuplicateProductNameException>(
                () => repository.AddAsync(second));
        }
        finally
        {
            if (first.Id > 0)
                await repository.DeleteAsync(first.Id);
        }
    }

    [Fact]
    public async Task GetByIdAsync_MissingId_ReturnsNull()
    {
        IProductRepository repository = CreateRepository();

        Product? result = await repository.GetByIdAsync(int.MaxValue);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_MissingProduct_ReturnsFalse()
    {
        IProductRepository repository = CreateRepository();

        Product missingProduct = new("Missing-Product", 10m, 5);
        missingProduct.SetId(int.MaxValue);

        bool result = await repository.UpdateAsync(missingProduct);

        Assert.False(result);
    }

    [Fact]
    public async Task FindByNameAsync_IgnoresCase()
    {
        IProductRepository repository = CreateRepository();

        string name = "Case-Test-" + Guid.NewGuid();

        Product product = new(name, 15m, 8);

        try
        {
            await repository.AddAsync(product);

            Product? result = await repository.FindByNameAsync(
                name.ToLowerInvariant());

            Assert.NotNull(result);
            Assert.Equal(product.Id, result.Id);
            Assert.Equal(product.Name, result.Name);
        }
        finally
        {
            if (product.Id > 0)
                await repository.DeleteAsync(product.Id);
        }
    }
    [Fact]
    public async Task AddAsync_ThenGetByIdAsync_PreservesProductData()
    {
        IProductRepository repository = CreateRepository();
        string name="Case-Test-"+Guid.NewGuid();
        Product product = new(name, 15m, 8);
        try
        {
           int id= await repository.AddAsync(product);
            Product? result = await repository.GetByIdAsync(id);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal(product.Name, result.Name);
            Assert.Equal(product.Price, result.Price);
            Assert.Equal(product.StockQty, result.StockQty);


        }
        finally
        {
            if (product.Id > 0)
                await repository.DeleteAsync(product.Id);

        }
    }
}
