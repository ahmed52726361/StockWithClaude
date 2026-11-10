using Inventory.DataAccess.Repositories;
using Inventory.DataAccess.UnitOfWork;
using Inventory.Domain.Products;
using Inventory.Domain.Repository_Interfaces;

class Program
{
    public static async Task TestTransactionAsync()
    {
        SqlUnitOfWork unitOfWork = new SqlUnitOfWork();
        IProductRepository productRepository=new SqlProductRepository(unitOfWork);
        Product? HandBag = await productRepository.FindByNameAsync("HandBag");
        Product? handBag = await productRepository.FindByNameAsync("HandBag");

        ArgumentNullException.ThrowIfNull(handBag);

        handBag.AddStock(3);
        await unitOfWork.BeginTransactionAsync();
        //using SqlConnection connection = SqlConnectionFactory.CreateConnection();
        //await connection.OpenAsync(); // how we used async and the connection is not a task ?
        //string query = "UPDATE Products SET StockQty=7 WHERE Name=@NewName";
        //string newName = "HandBag";
        //await using SqlTransaction transaction=(SqlTransaction)await connection.BeginTransactionAsync();
        //using SqlCommand command = new SqlCommand(query, connection, transaction);
        //command.Parameters.AddWithValue("@NewName",newName);


        try
        {
            await productRepository.UpdateAsync(handBag);
            await unitOfWork.CommitAsync();
            //await command.ExecuteNonQueryAsync();
            //await transaction.RollbackAsync();
            //await transaction.CommitAsync();

        }
        catch
        {
            await unitOfWork.RollbackAsync();
            //await transaction.RollbackAsync();
            throw;
        }
        finally
        {
            await unitOfWork.DisposeAsync();
        }

    }
    static async Task Main(string[] args)
    {
        for (int i = 0; i < 1000; i++)
        {
            await TestTransactionAsync();
            Console.WriteLine(i);
        }
    }
}