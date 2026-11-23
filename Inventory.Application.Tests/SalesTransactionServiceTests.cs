using Inventory.Application.Exceptions;
using Inventory.Application.Tests.FakeRepo;
using Inventory.Domain.Orders.Invoices;
using Inventory.Domain.Orders.Lines;
using Inventory.Domain.Products;
using Inventory.Domain.Products.Exceptions;
using Inventory.TestDoubles.Products;

namespace Inventory.Application.Tests
{
    public class SalesTransactionServiceTests
    {
        private readonly SalesTransactionService _salesTransactionService;
        private readonly FakeProductRepository _productRepository;
        private readonly FakeUnitOfWork _unitOfWork;
        private readonly FakeClientRepository _clientRepository;
        private readonly ClientService _clientService;
        private readonly FakeSaleInvoiceRepository _saleInvoiceRepository;
        private readonly FakeSalesOrderLineRepository _salesOrderLineRepository;

        public SalesTransactionServiceTests()
        {
            _productRepository = new FakeProductRepository();
            _unitOfWork = new FakeUnitOfWork();
            _clientRepository = new FakeClientRepository();
            _clientService=new ClientService(_clientRepository);
            _saleInvoiceRepository=new FakeSaleInvoiceRepository();
            _salesOrderLineRepository=new FakeSalesOrderLineRepository();
            _salesTransactionService = new(_productRepository, _unitOfWork
                , _clientService, _saleInvoiceRepository
                , _salesOrderLineRepository
                );

        }
        private async Task<int> AddClient()
        {
            return await _clientRepository.AddAsync(new(false, "Ahmed", "Sfax", "ahmed@gmail.com", "454545"));
        }
        private async Task<int> AddProduct(string ProductName,decimal Price,int stockQty)
        {
            return await _productRepository.AddAsync(new(ProductName,Price,stockQty));
        }
        [Fact]
        public async Task SellProductsAsync_ValidSale_SavesInvoiceAndLinesAndReducesStock()
        {
            //Arrange
            int ClientId=await AddClient();
            int FirstProductId = await AddProduct("Fridge", 10m, 5);
            int SecondProductId = await AddProduct("Tv", 10m, 15);
            List<SaleItem> items = new()
            {
                new SaleItem
                {
                    ProductID = FirstProductId,
                    ProductPrice=12m,
                    OrderQuantity=3
                },
                new SaleItem
                {
                    ProductID = SecondProductId,
                    ProductPrice=14m,
                    OrderQuantity=5
                }

            };
            //Act

            int SaleId=await _salesTransactionService.SellProductsAsync(ClientId,items);
            IReadOnlyList<SalesOrderLine>orderLines=await _salesOrderLineRepository.FindByInvoiceIDAsync(SaleId);
            Product? firsProduct=await _productRepository.GetByIdAsync(FirstProductId);
            Product? secondProduct = await _productRepository.GetByIdAsync(SecondProductId);
            var firstLine=orderLines.Single(x=>x.Product.Id==FirstProductId);
            var secondLine=orderLines.Single(x=>x.Product.Id== SecondProductId);
            SalesInvoice? salesInvoice=await _saleInvoiceRepository.GetByIdAsync(SaleId);
            //Assert
            Assert.True(_unitOfWork.Began);
            Assert.True(_unitOfWork.Committed);
            Assert.False(_unitOfWork.RolledBack);
            Assert.NotNull(firsProduct);
            Assert.NotNull(secondProduct);
            Assert.Equal( 2, firsProduct.StockQty);
            Assert.Equal( 10, secondProduct.StockQty);
            Assert.NotNull(salesInvoice);
            Assert.Equal(106m, _saleInvoiceRepository.SavedTotals[SaleId]);
            Assert.Equal(firsProduct.Id, firstLine.Product.Id);
            Assert.Equal(12m, firstLine.OrderPrice);
            Assert.Equal(3, firstLine.Qty);
            Assert.Equal(secondProduct.Id, secondLine.Product.Id);
            Assert.Equal(14m, secondLine.OrderPrice);
            Assert.Equal(5, secondLine.Qty);
        }
        [Fact]
        public async Task SellProductsAsync_unknownClient_ThrowClientNotFoundException()
        {
            //Arrange
            int FirstProductId = await AddProduct("Fridge", 10m, 5);
            int SecondProductId = await AddProduct("Tv", 10m, 15);
            List<SaleItem> items = new()
            {
                new SaleItem
                {
                    ProductID = FirstProductId,
                    ProductPrice=12m,
                    OrderQuantity=3
                },
                new SaleItem
                {
                    ProductID = SecondProductId,
                    ProductPrice=14m,
                    OrderQuantity=5
                }

            };
            //Assert
            await Assert.ThrowsAsync<ClientNotFoundException>(() => _salesTransactionService.SellProductsAsync(999, items));
            Assert.False(_unitOfWork.Began);

            
        }
        [Fact]
        public async Task SellProductsAsync_unknownProduct_ThrowProductNotFoundException()
        {
            //Arrange
            int cliendId =await AddClient();
            List<SaleItem> items = new()
            {
                new SaleItem
                {
                    ProductID = 1,
                    ProductPrice=12m,
                    OrderQuantity=3
                },
                new SaleItem
                {
                    ProductID = 2,
                    ProductPrice=14m,
                    OrderQuantity=5
                }

            };
            //Assert
            await Assert.ThrowsAsync<ProductNotFoundException>(() => _salesTransactionService.SellProductsAsync(cliendId, items));
            Assert.False(_unitOfWork.Began);


        }
        [Fact]
        public async Task SellProductsAsync_InsufficientStock_ThrowInsufficientStockException()
        {
            //Arrange
            int ClientId = await AddClient();
            int FirstProductId = await AddProduct("Fridge", 10m, 5);
            int SecondProductId = await AddProduct("Tv", 10m, 15);
            List<SaleItem> items = new()
            {
                new SaleItem
                {
                    ProductID = FirstProductId,
                    ProductPrice=12m,
                    OrderQuantity=3
                },
                new SaleItem
                {
                    ProductID = SecondProductId,
                    ProductPrice=14m,
                    OrderQuantity=16
                }

            };
            await Assert.ThrowsAsync<InsufficientStockException>(() => _salesTransactionService.SellProductsAsync(ClientId, items));
            Product? reloadedFirstProduct=await _productRepository.GetByIdAsync(FirstProductId);
            Assert.False(_unitOfWork.Began);
            IReadOnlyList<SalesInvoice> salesInvoice=await _saleInvoiceRepository.GetAllAsync();
            IReadOnlyList<SalesOrderLine> salesOrderLines=await _salesOrderLineRepository.GetAllAsync();
            Assert.Empty(salesInvoice); 
            Assert.Empty(salesOrderLines);
            Assert.Equal(5, reloadedFirstProduct.StockQty);
            

        }
        [Fact]
        public async Task SellProductsAsync_SameProductOnTwoLines_StockGoesDownByTheSum()
        {
            //Arrange
            int ClientId = await AddClient();
            int FirstProductId = await AddProduct("Fridge", 10m, 5);
            List<SaleItem> items = new()
            {
                new SaleItem
                {
                    ProductID = FirstProductId,
                    ProductPrice=12m,
                    OrderQuantity=3
                },
                new SaleItem
                {
                    ProductID = FirstProductId,
                    ProductPrice=14m,
                    OrderQuantity=1
                }

            };
            //Act

            int SaleId = await _salesTransactionService.SellProductsAsync(ClientId, items);
            IReadOnlyList<SalesOrderLine> orderLines = await _salesOrderLineRepository.FindByInvoiceIDAsync(SaleId);
            Product? firsProduct = await _productRepository.GetByIdAsync(FirstProductId);
            SalesInvoice? salesInvoice = await _saleInvoiceRepository.GetByIdAsync(SaleId);
            //Assert
            Assert.True(_unitOfWork.Began);
            Assert.True(_unitOfWork.Committed);
            Assert.False(_unitOfWork.RolledBack);
            Assert.NotNull(firsProduct);
            Assert.Equal(1, firsProduct.StockQty);
            Assert.NotNull(salesInvoice);
            Assert.Equal(50m, _saleInvoiceRepository.SavedTotals[SaleId]);
            Assert.Equal(2, orderLines.Count);
        }
        [Fact]
        public async Task SellProductsAsync_SameProductOnTwoLinesWithOrderGreatThanStock_ThrowInsufficientStockException()
        {
            //Arrange
            int ClientId = await AddClient();
            int FirstProductId = await AddProduct("Fridge", 10m, 5);
            List<SaleItem> items = new()
            {
                new SaleItem
                {
                    ProductID = FirstProductId,
                    ProductPrice=12m,
                    OrderQuantity=3
                },
                new SaleItem
                {
                    ProductID = FirstProductId,
                    ProductPrice=14m,
                    OrderQuantity=3
                }

            };
            await Assert.ThrowsAsync<InsufficientStockException>(() => _salesTransactionService.SellProductsAsync(ClientId, items));
            Assert.False(_unitOfWork.Began);
            IReadOnlyList<SalesInvoice> salesInvoice = await _saleInvoiceRepository.GetAllAsync();
            IReadOnlyList<SalesOrderLine> salesOrderLines = await _salesOrderLineRepository.GetAllAsync();
            Assert.Empty(salesInvoice);
            Assert.Empty(salesOrderLines);


        }
        [Fact]
        public async Task SellProductsAsync_StockTakenByAnotherSaleBeforeSaving_RollsBack()
        {
            //Arrange
            int clientId = await AddClient();
            int productId = await AddProduct("Fridge", 10m, 5);
            List<SaleItem> items = new()
    {
        new SaleItem { ProductID = productId, ProductPrice = 12m, OrderQuantity = 3 }
    };

            // Phase 1 passes (5 >= 3), but the conditional update in Phase 2 reports "0 rows"
            _productRepository.ForceStockReductionToFail = true;
             
            //Act + Assert
            await Assert.ThrowsAsync<InsufficientStockException>(
                () => _salesTransactionService.SellProductsAsync(clientId, items));

            Assert.True(_unitOfWork.Began);        // the transaction really started
            Assert.True(_unitOfWork.RolledBack);   // and the service asked for a rollback
            Assert.False(_unitOfWork.Committed);
           
        }
    }
}
