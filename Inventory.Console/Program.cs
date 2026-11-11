using Inventory.Application;
using Inventory.DataAccess.Repositories;
using Inventory.DataAccess.UnitOfWork;
using Inventory.Domain.Products;
using Inventory.Domain.Repository_Interfaces;

class Program
{
    public static async Task<int> TestTransactionAddSaleInvoiceWithOrderLines()
    {
        SqlUnitOfWork unitOfWork = new SqlUnitOfWork();
        SqlClientRepository sqlClientRepository=new(unitOfWork);
        SqlProductRepository sqlProductRepository=new(unitOfWork);
        SqlSaleInvoiceRepository sqlSaleInvoiceRepository=new(unitOfWork);
        SqlSalesOrderLineRepository sqlSalesOrderLineRepository = new(unitOfWork);
        CompletedSaleInvoiceWithOrderLines completedSaleInvoiceWithOrderLines = new(unitOfWork, sqlSaleInvoiceRepository, sqlProductRepository, sqlClientRepository, sqlSalesOrderLineRepository);
        List<ListProductsToSale> products = new()
        {
            new ListProductsToSale
            {
                ProductID = 1,
                OrderQuantity = 2,
                ProductPrice = 15.99m
            },
            new ListProductsToSale
            {
                ProductID = 3,
                OrderQuantity = 1,
                ProductPrice = 24.50m
            },
            new ListProductsToSale
            {
                ProductID = 5,
                OrderQuantity = 2,
                ProductPrice = 9.75m
            }
        };
       return await completedSaleInvoiceWithOrderLines.CreateSaleInvoiceWithOrderLines(1, products);

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
        if (await TestTransactionAddSaleInvoiceWithOrderLines()>0)
        {
            Console.WriteLine("Transaction works successfully");
        }
        else
        {
            Console.WriteLine("Transaction not working");
            
        }
    }
}