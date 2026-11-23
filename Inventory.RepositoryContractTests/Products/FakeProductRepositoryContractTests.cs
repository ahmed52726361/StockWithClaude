using Inventory.Domain.Repository_Interfaces;
using Inventory.TestDoubles.Products;

namespace Inventory.RepositoryContractTests.Products;

public class FakeProductRepositoryContractTests : ProductRepositoryContract
{
    protected override IProductRepository CreateRepository()
    {
        return new FakeProductRepository();
    }
}