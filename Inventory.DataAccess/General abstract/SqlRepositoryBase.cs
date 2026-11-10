using Inventory.DataAccess.Database;
using Inventory.DataAccess.UnitOfWork;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Net.Sockets;

namespace Inventory.DataAccess.General_abstract
{
    public abstract class SqlRepositoryBase<T> 
    {
        private SqlUnitOfWork _unitOfWork;
        protected  SqlRepositoryBase(SqlUnitOfWork? unitOfWork=null)
        {
            _unitOfWork = unitOfWork;
        }
        //protected  SqlRepositoryBase()
        //{
        //    // I tought that wa can add another one like this if there is no transaction
        //}

        protected abstract T BuildTable(SqlDataReader reader);
        private SqlCommand CreateCommand( string query, List<QueryParameter> parameters)
        {
            SqlConnection connection;
            SqlCommand command;
            if (_unitOfWork!=null&&_unitOfWork.HasActiveTransaction)
            {
                connection=_unitOfWork._connection;
                command = new(query, connection, _unitOfWork._transaction);
            }
            else
            {
                connection=SqlConnectionFactory.CreateConnection();
                command = new(query, connection);
            }
            foreach (var parameter in parameters)
            {
                command.Parameters
                    .Add(parameter.Name, parameter.Type)
                    .Value = parameter.Value ?? DBNull.Value;
            }
            return command;

        }
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
            using SqlDataReader reader = await CreateCommand(query, parameters).ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(map(reader));
            }
            return result;

        }
        protected async Task<int> ExecuteAsync(string query, List<QueryParameter> parameters)
        {
            return await CreateCommand(query, parameters).ExecuteNonQueryAsync();
        }
        protected async Task<TResult> ExecuteScalarAsync<TResult>(string query, List<QueryParameter> parameters)
        {
            object? result = await CreateCommand(query,parameters).ExecuteScalarAsync();
            if (result == null || result == DBNull.Value)
            {
                throw new InvalidOperationException("The query did not return a value.");
            }

            // I asked AI and tell me that it prefered to use Convert.ChangeType to handle the conversion to TResult, but I think it is better to use a direct cast since we expect the result to be of type TResult. If the result is not of type TResult, it will throw an InvalidCastException, which is more appropriate in this context. so what do you think ??
            return (TResult)result;
        }

    }
}
