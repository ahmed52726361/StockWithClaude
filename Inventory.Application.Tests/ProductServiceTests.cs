using Inventory.Application.Exceptions;
using Inventory.Domain.Products;

namespace Inventory.Application.Tests
{

    public class ProductServiceTests
    {
        private readonly FakeProductRepository _repository;
        private readonly ProductService _productService;
        private async Task<int> AddHandBagAsync(decimal price = 10m, int stock = 7)
        {
            return await _repository.AddAsync(new Product("HandBag", price, stock));
        }
        public ProductServiceTests()
        {
            _repository = new FakeProductRepository();
            _productService=new ProductService(_repository);
        }

        [Fact]
        public async Task AddProductAsync_NewName_ReturnsIdOfSavedProduct()
        {
            int id =await _productService.AddProductAsync("HandBag", 10m, 7);
            Product? saved=await _repository.GetByIdAsync(id);
            Assert.NotNull(saved);
            Assert.Equal("HandBag",saved.Name);
            Assert.Equal(10m, saved.Price);
            Assert.Equal(7,saved.StockQty);
        }
        [Fact]
        public async Task AddProductAsync_DuplicateName_ThrowsDuplicateProductNameException()
        {
            await _productService.AddProductAsync("HandBag", 10m, 7);
            await Assert.ThrowsAnyAsync<DuplicateProductNameException>(() => _productService.AddProductAsync("HandBag", 10m, 7));
        }
        [Fact]
        public async Task AddProductAsync_SameNameDifferentCase_ThrowsDuplicateProductNameException()
        {
            await _productService.AddProductAsync("handBag", 10m, 7);
            await Assert.ThrowsAnyAsync<DuplicateProductNameException>(() => _productService.AddProductAsync("HandBag", 10m, 7));
        }
        [Fact]
        public async Task AddProductAsync_EmptyName_ThrowsArgumentException()
        {
            await Assert.ThrowsAnyAsync<ArgumentException>(() => _productService.AddProductAsync(" ", 10m, 7));
        }
        [Fact]
        public async Task RestockProductAsync_ProductDoesNotExist_ThrowsProductNotFoundException()
        {
            await Assert.ThrowsAnyAsync<ProductNotFoundException>(async () =>await _productService.RestockProductAsync(1, 1));
        }
        [Fact]
        public async Task RestockProductAsync_AddQuantity_ReturnsCorrectedQuantityInStock()
        {
            int id =await AddHandBagAsync();
            bool result = await _productService.RestockProductAsync(id, 3);
            Product? Reloaded=await _repository.GetByIdAsync(id);
            Assert.True(result);
            Assert.NotNull(Reloaded);
            Assert.Equal(10,Reloaded.StockQty);
        }
        [Fact]
        public async Task RestockProductAsync_NegativeQuantity_DoesNotChangeStoredStock()
        {
            int id = await AddHandBagAsync();   // stock is 7

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => _productService.RestockProductAsync(id, -1));

            Product? reloaded = await _repository.GetByIdAsync(id);
            Assert.NotNull(reloaded);
            Assert.Equal(7, reloaded.StockQty);   // nothing was saved
        }
        [Fact]
        public async Task ChangeProductPriceAsync_ChangedSucceffully_ReturnNewPrice()
        {
            int id = await AddHandBagAsync();
            bool result =await _productService.ChangeProductPriceAsync(id, 3m);
            Product? Reloaded=await _repository.GetByIdAsync(id);
            Assert.True(result);
            Assert.NotNull(Reloaded);
            Assert.Equal(3m,Reloaded.Price);
        }
        [Fact]
        public async Task UpdateProductAsync_ProductNotExist_throwProductNotFoundException()
        {
            Product product = new("test", 14m, 7);
            await Assert.ThrowsAnyAsync<ProductNotFoundException>(async ()=>await _productService.UpdateProductAsync(product));
        }



    }
}
