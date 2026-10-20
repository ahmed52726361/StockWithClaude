using Inventory.DataAccess.Database;
using Inventory.Products;
using Inventory.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Inventory.General_Interfaces_and_abstract.General_abstract
{
    internal abstract class SqlRepositoryBase<T> 
    {
        protected const string BaseSelect = @"
            SELECT
                sol.SalesOrderLineID,
                sol.OrderQty,
                sol.OrderPrice,

                si.SalesInvoiceID,
                si.InvoiceDate,
                si.TotalAmount,
                si.PaidStatus,

                c.ClientID,
                c.Name AS ClientName,
                c.Address,
                c.Email,
                c.PhoneNumber,
                c.PromotionAccept,

                p.ProductID,
                p.Name AS ProductName,
                p.StockQty,
                p.Price

            FROM SalesOrderLines sol
            JOIN SalesInvoices si ON sol.SalesInvoiceID = si.SalesInvoiceID
            JOIN Clients c ON si.ClientID = c.ClientID
            JOIN Products p ON sol.ProductID = p.ProductID";

        protected abstract T BuildTable(SqlDataReader reader);
        protected async Task<T> QuerySingleAsync(string query, List<QueryParameter> parameters, Func<SqlDataReader, T> map)
        {
            List<T> result=await QueryAsync(query, parameters, map);
            return result.FirstOrDefault();
        }
        protected async Task<List<T>> QueryAsync(string query,List<QueryParameter> parameters, Func<SqlDataReader, T> map)
        {
            var result= new List<T>();
            using SqlConnection connection=SqlConnectionFactory.CreateConnection();
            using SqlCommand command=new SqlCommand(query, connection);
            foreach(QueryParameter parameter in parameters)
            {
                command.Parameters.Add(parameter.Name,parameter.Type).Value = parameter.Value;
            }
            await connection.OpenAsync();
            using SqlDataReader reader=await command.ExecuteReaderAsync();
            while(await reader.ReadAsync())
            {
                result.Add(map(reader));
            }
            return result;

        }
        protected async Task<int> ExecuteAsync(string query, List<QueryParameter> parameters)
        {
            using SqlConnection connection = SqlConnectionFactory.CreateConnection();
            using SqlCommand command=new SqlCommand(query,connection);
            foreach( var parameter in parameters)
            {
                command.Parameters.Add(parameter.Name, parameter.Type).Value = parameter.Value;
            }
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync();
        }
    }
}
