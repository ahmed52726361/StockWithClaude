using Inventory.Products;

namespace Inventory.Domain.Tests
{
    public class ProductDomainTests
    {
        #region constructor

        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData(null)]
        public void Constructor_EmptyName_ThrowsArgumentException(string? empty)
        {
            Assert.Throws<ArgumentException>(()=>new Product(empty!, 10m, 7));
        }
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Constructor_PriceLessThanOrEqualToZero_ThrowsArgumentOutOfRangeException(decimal price)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Product("Test", price,5));
        }
        [Fact]
        public void Constructor_NegativeQuantity_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Product("Test", 10m,-1));
        }
        [Fact]
        public void Constructor_ZeroQuantity_CreatesProduct()
        {
            Product product = new("Test", 10m, 0);

            Assert.Equal(0, product.StockQty);
        }
        [Fact]
        public void Constructor_NameWithSpaces_TrimsName()
        {
            Product product = new("  Test  ", 10m, 5);

            Assert.Equal("Test", product.Name);
        }
        #endregion
        #region SetId
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void SetID_IDisLessOrEqualToZero_ArgumentOutOfRangeException(int id)
        {
            Product product = new("Test", 10m, 5);
            Assert.Throws<ArgumentOutOfRangeException>(() => product.SetId(id));
        }

        [Fact]
        public void SetId_ValidId_SetsId()
        {
            Product product = new("Test", 10m, 5);

            product.SetId(1);

            Assert.Equal(1, product.Id);
        }
        [Fact]
        public void SetId_SetAnotherIdSameObject_InvalidOperationException()
        {
            Product product = new("Test", 10m, 5);

            product.SetId(1);

            Assert.Throws<InvalidOperationException>(() => product.SetId(2));
        }
        #endregion
        #region Renaming
        [Theory]
        [InlineData("  Bag ")]
        [InlineData("Bag")]
        public void Rename_TrimmedNameOrNormalOne_NameUpdatedSuccefullyWithoutSpaces(string newName)
        {
            Product product = new("Test", 10m, 5);
            product.Rename(newName);
            Assert.Equal("Bag", product.Name);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData(null)]
        public void Rename_EmptyName_ThrowArgumentException(string? emptyName)
        {
            Product product = new("Test", 10m, 5);

            Assert.Throws<ArgumentException>(
                () => product.Rename(emptyName!));
        }
        #endregion
        #region AddQtyStock
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
        [Fact]
        public void AddStock_QuantityCausesOverflow_ThrowsOverflowException()
        {
            Product product = new("Test", 10m, int.MaxValue);
            Assert.Throws<OverflowException>(()=>product.AddStock(1));
        }
        
        #endregion
        #region Sales
        [Fact]
        public void Sale_ValidQuantity_ReducesStockQuantity()
        {
            //Arrange
            Product product=new("Test",10m,5);
            //Act
            product.Sale(2);
            // Assert
            Assert.Equal(3, product.StockQty);
        }
        [Fact]
        public void Sale_QuantityEqualToStock_ReducesStockToZero()
        {
            //Arrange
            Product product=new("Test",10m,5);
            //Act
            product.Sale(5);
            // Assert
            Assert.Equal(0, product.StockQty);
        }
        [Fact]
        public void Sale_QuantityGreaterThanStock_ThrowsInvalidOperationException()
        {
            Product product = new("Test", 10m, 5);
            Assert.Throws<InvalidOperationException>(()=>product.Sale(6));
        }
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Sale_QuantityLessThanOrEqualToZero_ThrowsArgumentOutOfRangeException(int qty)
        {
            Product product = new("Test", 10m, 5);
            Assert.Throws<ArgumentOutOfRangeException>(()=>product.Sale(qty));
        }

        #endregion
        #region ChangePrice
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ChangePrice_PriceLessOrEqualToZero_ArgumentOutOfRangeException(decimal newPrice)
        {

            Product product = new Product("Test", 10m, 5);
            Assert.Throws<ArgumentOutOfRangeException>(() =>product.ChangePrice(newPrice));
        }
        [Fact]
        public void ChangePrice_ValidPrice_UpdatesPrice()
        {
            Product product = new("Test", 10m, 5);

            product.ChangePrice(25m);

            Assert.Equal(25m, product.Price);
        }

        #endregion



    }
}
