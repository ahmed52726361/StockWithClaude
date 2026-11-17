using Inventory.Application;
using Inventory.DataAccess.Repositories;
using Inventory.DataAccess.UnitOfWork;
using Inventory.Domain.Products;
using Inventory.Domain.Repository_Interfaces;

class Program
{
    private static SalesTransactionService CreateSalesTransactionService(SqlUnitOfWork unitOfWork)
    {
        SqlClientRepository sqlClientRepository = new(unitOfWork);
        SqlProductRepository sqlProductRepository = new(unitOfWork);
        SqlSaleInvoiceRepository sqlSaleInvoiceRepository = new(unitOfWork);
        SqlSalesOrderLineRepository sqlSalesOrderLineRepository = new(unitOfWork);
        ClientService clientService = new(sqlClientRepository);
        return new SalesTransactionService(
            sqlProductRepository,
            unitOfWork,
            clientService,
            sqlSaleInvoiceRepository,
            sqlSalesOrderLineRepository);
    }
    public static async Task<int> ServiceA()
    {
        SalesTransactionService salesTransactionService =CreateSalesTransactionService(new SqlUnitOfWork());
        List<SaleItem> products = new()
        {
            new SaleItem
            {
                ProductID = 1,
                OrderQuantity = 6,
                ProductPrice = 15.99m
            }
        };

        int ID =
            await salesTransactionService.SellProductsAsync(1, products);

        Console.WriteLine($"Second Invoice: {ID}");

        return ID;
    }
    public static async Task<int> ServiceB()
    {
        SalesTransactionService salesTransactionService =CreateSalesTransactionService(new SqlUnitOfWork());
        List<SaleItem> products = new()
        {
            new SaleItem
            {
                ProductID = 1,
                OrderQuantity = 6,
                ProductPrice = 15.99m
            }
        };

        int ID =
            await salesTransactionService.SellProductsAsync(1, products);

        Console.WriteLine($"Second Invoice: {ID}");

        return ID;
    }

    public static async Task TestTransactionAsync()
    {
        SqlUnitOfWork unitOfWork = new SqlUnitOfWork();
        IProductRepository productRepository = new SqlProductRepository(unitOfWork);

        await unitOfWork.BeginTransactionAsync();

        try
        {
            Product? handBag = await productRepository.FindByNameAsync("HandBag");
            ArgumentNullException.ThrowIfNull(handBag);

            handBag.AddStock(3);
            await productRepository.UpdateAsync(handBag);
            Product? updatedHandBag = await productRepository.FindByNameAsync("HandBag");

            Console.WriteLine($"Stock inside transaction: {updatedHandBag?.StockQty}");
            Console.WriteLine("Press ENTER after checking SSMS...");
            Console.ReadLine();

            await unitOfWork.RollbackAsync();
        }
        catch
        {
            await unitOfWork.RollbackAsync();
            throw;
        }
        finally
        {
            await unitOfWork.DisposeAsync();
        }
        
    }
    public static async Task TestInsertRollbackAsync()
    {
        SqlUnitOfWork unitOfWork= new SqlUnitOfWork();
        SqlProductRepository productRepository= new SqlProductRepository(unitOfWork);
        await unitOfWork.BeginTransactionAsync( );
        try
        {
            Product Phone = new Product("Phone", 17.5m,0);
            int id= await productRepository.AddAsync(Phone);
            Console.WriteLine($"Phone Id is {id}");
            await unitOfWork.RollbackAsync();
        }
        catch
        {
            await unitOfWork.RollbackAsync();
            throw;
        }
        finally
        {
            await unitOfWork.DisposeAsync();
        }
    }
    static async Task Main(string[] args)
    {
        await Task.WhenAll(ServiceA(), ServiceB());
    }
}