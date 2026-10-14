using Inventory.DataAccess.Database;
using Inventory.General_Interfaces.Repository_Interfaces;
using Inventory.Products;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.DataAccess.Repositories
{
    internal class SqlProductRepository : IProductRepository
    {
          
        public async Task AddAsync(Product item)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(item));
            using SqlConnection connection = SqlConnectionFactory.CreateConnection();
            
                string query = @"INSERT INTO Products (Name, StockQty, Price)
                             VALUES (@Name, @StockQty, @Price)";
              using SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", item.Name);
                command.Parameters.AddWithValue("@StockQty", item.StockQty);
                command.Parameters.AddWithValue("@Price", item.Price);
            /* should i use this code instead ?
            command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = item.Name;
            command.Parameters.Add("@StockQty", SqlDbType.Int).Value = item.StockQty;
            command.Parameters.Add("@Price", SqlDbType.Decimal).Value = item.Price;
            */
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(id, 0, nameof(id));
            using SqlConnection connection= SqlConnectionFactory.CreateConnection();
            string query = @"DELETE FROM Products WHERE ProductID = @id";
            using SqlCommand command = new SqlCommand(query,connection);  
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;
            await connection.OpenAsync();
            int rowAffected=await command.ExecuteNonQueryAsync();
            return rowAffected > 0;
        }

        public async Task<Product?> FindByNameAsync(string name)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);
            using SqlConnection connection = SqlConnectionFactory.CreateConnection();
            string query = "SELECT ProductID, Name, StockQty, Price FROM Products WHERE Name = @name";
            using SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@name", name);
            await connection.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                string Name = (string)reader["Name"];
                int Qty = (int)reader["StockQty"];
                decimal price = (decimal)reader["Price"];
                return new Product(Name, price, Qty);
            }
            return null;
        }

        public async Task<IReadOnlyList<Product>> FindLowStockAsync(int threshold)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(threshold, 0);

            List<Product> products = new();

            using SqlConnection connection = SqlConnectionFactory.CreateConnection();

            string query = @"SELECT ProductID, Name, StockQty, Price
                     FROM Products
                     WHERE StockQty <= @threshold";

            using SqlCommand cmd = new SqlCommand(query, connection);

            cmd.Parameters.Add("@threshold", SqlDbType.Int).Value = threshold;

            await connection.OpenAsync();

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                string name = (string)reader["Name"];
                int qty = (int)reader["StockQty"];
                decimal price = (decimal)reader["Price"];

                products.Add(new Product(name, price, qty));
            }

            return products;
        }
        public async Task<IReadOnlyList<Product>> GetAllAsync()
        {
            List<Product> products = new();

            using SqlConnection connection = SqlConnectionFactory.CreateConnection();

            string query = @"SELECT ProductID, Name, StockQty, Price
                     FROM Products";

            using SqlCommand cmd = new SqlCommand(query, connection);

            await connection.OpenAsync();

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                string name = (string)reader["Name"];
                int qty = (int)reader["StockQty"];
                decimal price = (decimal)reader["Price"];

                products.Add(new Product(name, price, qty));
            }

            return products;
        }
        public async Task<Product?> GetByIdAsync(int id)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(id, 0, nameof(id));
            using SqlConnection connection= SqlConnectionFactory.CreateConnection();
            string query = "SELECT ProductID, Name, StockQty, Price FROM Products WHERE ProductID = @Id";
            using SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id; await connection.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                string Name = (string)reader["Name"];
                int Qty = (int)reader["StockQty"];
                decimal price = (decimal)reader["Price"];
                return new Product(Name, price, Qty);
            }
            return null;
        }
    }
}
