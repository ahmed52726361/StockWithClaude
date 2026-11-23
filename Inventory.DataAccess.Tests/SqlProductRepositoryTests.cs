
using Inventory.DataAccess.Repositories;
using Inventory.DataAccess.UnitOfWork;
using Inventory.Domain.Products;
using Inventory.Domain.Products.Exceptions;

namespace Inventory.DataAccess.Tests.Repositories;

public class SqlProductRepositoryTests : IAsyncLifetime
{
    private readonly SqlUnitOfWork _unitOfWork;
    private readonly SqlProductRepository _repository;

    public SqlProductRepositoryTests()
    {
        _unitOfWork = new SqlUnitOfWork();
        _repository = new SqlProductRepository(_unitOfWork);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _unitOfWork.DisposeAsync();
    }
    //[Fact]
    //public async Task AddProductAsync_NewName_ReturnsIdOfSavedProduct()
    //{
    //    string name = "TestingGuid";
    //    Product product = new(name, 10m, 7);
    //    int id = await _repository.AddAsync(product);
    //    Assert.True(id > 0);
    //    //Product? saved = await _repository.GetByIdAsync(id);
    //    //Assert.NotNull(saved);
    //    //Assert.Equal(name, saved.Name);
    //    //Assert.Equal(10m, saved.Price);
    //    //Assert.Equal(7, saved.StockQty);
    //}

    [Fact]
    public async Task AddAsync_DuplicateName_ThrowsDuplicateProductNameException()
    {
        // Arrange
        string name = "Duplicate-Test-" + Guid.NewGuid();

        Product first = new(name, 10m, 5);
        Product second = new(name, 20m, 10);

        try
        {
            await _repository.AddAsync(first);

            Assert.True(first.Id > 0, $"Expected a valid ID, but got {first.Id}");
            // Act & Assert
            await Assert.ThrowsAsync<DuplicateProductNameException>(
                () => _repository.AddAsync(second));
        }
        finally
        {
            // Cleanup
            await _repository.DeleteAsync(first.Id);
        }
    }

    [Fact]
    public async Task GetByIdAsync_MissingId_ReturnsNull()
    {
        // Arrange
        int missingId = int.MaxValue;

        // Act
        Product? result = await _repository.GetByIdAsync(missingId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_MissingProduct_ReturnsFalse()
    {
        // Arrange
        Product missingProduct = new("Missing-Product", 10m, 5);
        missingProduct.SetId(int.MaxValue);

        // Act
        bool result = await _repository.UpdateAsync(missingProduct);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task FindByNameAsync_IgnoresCase()
    {
        // Arrange
        string name = "Case-Test-" + Guid.NewGuid();

        Product product = new(name, 15m, 8);

        try
        {
            await _repository.AddAsync(product);

            // Act
            Product? result = await _repository.FindByNameAsync(
                name.ToLowerInvariant());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product.Id, result.Id);
            Assert.Equal(product.Name, result.Name);
        }
        finally
        {
            // Cleanup
            await _repository.DeleteAsync(product.Id);
        }
    }
}
