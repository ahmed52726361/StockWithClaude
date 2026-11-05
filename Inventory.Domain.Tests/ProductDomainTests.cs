using Inventory.Products;

namespace Inventory.Domain.Tests
{
    public class ProductDomainTests
    {
        [Fact]
        public void AddProduct_EmptyName_ThrowArgumentException()
        {
            Assert.ThrowsAny<ArgumentException>(()=>new Product(" ", 10m, 7));
        }
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void AddProduct_PriceLessOrEqualToZero_ArgumentOutOfRangeException(decimal price)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Product("Test", price,5));
        }
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ChangePrice_PriceLessOrEqualToZero_ArgumentOutOfRangeException(decimal newPrice)
        {

            Product product = new Product("Test", 10m, 5);
            Assert.Throws<ArgumentOutOfRangeException>(() =>product.ChangePrice(newPrice));
        }
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void AddStock_QtyLessThanOrEqualToZero_ThrowsArgumentOutOfRangeException(int qty)
        {
            
            Product product=new Product("Test", 10m,5);
            Assert.Throws<ArgumentOutOfRangeException>(()=>product.AddStock(qty));
        }
        [Fact]
        public void AddStock_QuantityGreaterThanZero_UpdatesStockQuantity()
        {
            //Arrange
            Product product=new("Test",10m,5);
            //Act
            product.AddStock(5);
            // Assert
            Assert.Equal(10, product.StockQty);
        }
        
    }
}
