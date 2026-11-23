
using Inventory.DataAccess.Repositories;
using Inventory.DataAccess.UnitOfWork;
using Inventory.Domain.Repository_Interfaces;

namespace Inventory.RepositoryContractTests.Products;

public class SqlProductRepositoryContractTests
    : ProductRepositoryContract, IAsyncLifetime
{
    private readonly SqlUnitOfWork _unitOfWork;

    public SqlProductRepositoryContractTests()
    {
        _unitOfWork = new SqlUnitOfWork();
    }

    protected override IProductRepository CreateRepository()
    {
        return new SqlProductRepository(_unitOfWork);
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _unitOfWork.DisposeAsync();
    }
}
