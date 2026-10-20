using Inventory.DataAccess.Database;
using Inventory.General_Interfaces.Repository_Interfaces;
using Inventory.General_Interfaces_and_abstract.General_abstract;
using Inventory.Products;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Inventory.DataAccess.Repositories
{
    internal class SqlProductRepository :SqlRepositoryBase<Product>, IProductRepository
    {
        protected override Product BuildTable(SqlDataReader reader)
        {
            int Id = (int)reader["ProductID"];
            string Name = (string)reader["Name"];
            int Qty = (int)reader["StockQty"];
            decimal price = (decimal)reader["Price"];
            Product product = new(Name, price, Qty);
            product.SetId(Id);
            return product;
        }
        public async Task AddAsync(Product item)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(item));
            
                string query = @"INSERT INTO Products (Name, StockQty, Price)
                             VALUES (@Name, @StockQty, @Price)";
            List<QueryParameter> paramaters = new List<QueryParameter>();
            paramaters.Add(new QueryParameter("@Name",SqlDbType.NVarChar,item.Name));
            paramaters.Add(new QueryParameter("@StockQty", SqlDbType.Int,item.StockQty));
            paramaters.Add(new QueryParameter("@Price", SqlDbType.Decimal,item.Price));
            await ExecuteAsync(query, paramaters);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(id, 0, nameof(id));
            string query = @"DELETE FROM Products WHERE ProductID = @id";
            List<QueryParameter > paramaters = new List<QueryParameter>();
            paramaters.Add(new QueryParameter("@id", SqlDbType.Int,id));
            return await ExecuteAsync(query, paramaters) > 0;
        }

        public async Task<Product> FindByNameAsync(string name)
        {
            ArgumentException.ThrowIfNullOrEmpty(name); 
            string query = "SELECT ProductID, Name, StockQty, Price FROM Products WHERE Name = @name";
            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("@Name",SqlDbType.NVarChar,name));
            return await QuerySingleAsync(query, parameters,BuildTable);
        }

        public async Task<IReadOnlyList<Product>> FindLowStockAsync(int threshold)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(threshold, 0);


            string query = @"SELECT ProductID, Name, StockQty, Price
                     FROM Products
                     WHERE StockQty <= @threshold";
            List<QueryParameter> parameters = new List<QueryParameter>(); 
            parameters.Add(new QueryParameter("@threshold", SqlDbType.Int, threshold));
            return await QueryAsync(query, parameters, BuildTable);
        }
        public async Task<IReadOnlyList<Product>> GetAllAsync()
        {

            string query = @"SELECT ProductID, Name, StockQty, Price
                     FROM Products";
            return await base.QueryAsync(query, new List<QueryParameter>(), BuildTable);


        }
        public async Task<Product?> GetByIdAsync(int id)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(id, 0, nameof(id));
            string query = "SELECT ProductID, Name, StockQty, Price FROM Products WHERE ProductID = @Id";
            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("@Id", SqlDbType.NVarChar, id));
            return await QuerySingleAsync(query, parameters, BuildTable);

        }

    }
}
