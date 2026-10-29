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
        protected abstract T BuildTable(SqlDataReader reader);
        protected async Task<T?> QuerySingleAsync(
            string query,
            List<QueryParameter> parameters,
            Func<SqlDataReader, T> map)
        {
            List<T> results = await QueryAsync(query, parameters, map);

            return results.FirstOrDefault();
        }
        protected async Task<List<T>> QueryAsync(string query,List<QueryParameter> parameters, Func<SqlDataReader, T> map)
        {
            var result= new List<T>();
            using SqlConnection connection=SqlConnectionFactory.CreateConnection();
            using SqlCommand command=new SqlCommand(query, connection);
            foreach(QueryParameter parameter in parameters)
            {
                command.Parameters
                    .Add(parameter.Name, parameter.Type)
                    .Value = parameter.Value ?? DBNull.Value;
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
                command.Parameters
                    .Add(parameter.Name, parameter.Type)
                    .Value = parameter.Value ?? DBNull.Value;
            }
            await connection.OpenAsync();
            // return the number of rows affected by the command
            return await command.ExecuteNonQueryAsync();
        }
        protected async Task<TResult> ExecuteScalarAsync<TResult>(string query, List<QueryParameter> parameters)
        {
            using SqlConnection connection = SqlConnectionFactory.CreateConnection();
            using SqlCommand command = new SqlCommand(query, connection);
            foreach (var parameter in parameters)
            {
                command.Parameters
                    .Add(parameter.Name, parameter.Type)
                    .Value = parameter.Value ?? DBNull.Value;
            }
            await connection.OpenAsync();
            object? result = await command.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value)
            {
                throw new InvalidOperationException("The query did not return a value.");
            }

            // I asked AI and tell me that it prefered to use Convert.ChangeType to handle the conversion to TResult, but I think it is better to use a direct cast since we expect the result to be of type TResult. If the result is not of type TResult, it will throw an InvalidCastException, which is more appropriate in this context. so what do you think ??
            return (TResult)result;
        }

    }
}
