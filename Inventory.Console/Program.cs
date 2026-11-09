using Inventory.DataAccess.Database;
using Inventory.DataAccess.Repositories;
using Microsoft.Data.SqlClient;
using Inventory.Domain.Repository_Interfaces;
using System;
using System.Threading.Tasks;

class Program
{
    public static async Task TestTransactionAsync()
    {
        IProductRepository productRepository=new SqlProductRepository();
        using SqlConnection connection = SqlConnectionFactory.CreateConnection();
        await connection.OpenAsync(); // how we used async and the connection is not a task ?
        string query = "UPDATE Products SET StockQty=7 WHERE Name=@NewName";
        string newName = "HandBag";
        await using SqlTransaction transaction=(SqlTransaction)await connection.BeginTransactionAsync();
        using SqlCommand command = new SqlCommand(query, connection, transaction);
        command.Parameters.AddWithValue("@NewName",newName);

        try
        {
            await command.ExecuteNonQueryAsync();
            //await transaction.RollbackAsync();
            await transaction.CommitAsync();

        }catch
        {
            await transaction.RollbackAsync();
            throw;
        }

    }
    static async Task Main(string[] args)
    {
        await TestTransactionAsync();
    }
}