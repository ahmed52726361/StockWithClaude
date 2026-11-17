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
}