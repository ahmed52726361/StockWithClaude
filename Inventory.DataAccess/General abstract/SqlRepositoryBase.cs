using Inventory.DataAccess.Database;
using Inventory.DataAccess.UnitOfWork;
using Microsoft.Data.SqlClient;

namespace Inventory.DataAccess.General_abstract
{
    public abstract class SqlRepositoryBase<T>
    {
        private readonly SqlUnitOfWork? _unitOfWork;

        protected SqlRepositoryBase(SqlUnitOfWork? unitOfWork = null)
        {
            _unitOfWork = unitOfWork;
        }

        protected abstract T BuildTable(SqlDataReader reader);

        // The ONE place that decides which connection to use, opens it,
        // runs the work, and cleans up.
        private async Task<TResult> RunAsync<TResult>(
            string query,
            List<QueryParameter> parameters,
            Func<SqlCommand, Task<TResult>> action)
        {
            SqlConnection? connection;
            SqlTransaction? transaction;
            bool ownsConnection;

            SqlUnitOfWork? unitOfWork = _unitOfWork;
            if (unitOfWork != null && unitOfWork.HasActiveTransaction)
            {
                // Shared: already open, owned by the unit of work. Never open or dispose it.
                connection = unitOfWork._connection;
                transaction = unitOfWork._transaction;
                ownsConnection = false;
            }
            else
            {
                // Private: we open it and we dispose it.
                connection = SqlConnectionFactory.CreateConnection();
                transaction = null;
                ownsConnection = true;
            }

            try
            {
                if (ownsConnection)
                    await connection.OpenAsync();

                await using SqlCommand command = new(query, connection, transaction);
                foreach (QueryParameter parameter in parameters)
                {
                    command.Parameters
                        .Add(parameter.Name, parameter.Type)
                        .Value = parameter.Value ?? DBNull.Value;
                }

                return await action(command);
            }   // the command is disposed here
            finally
            {
                if (ownsConnection)
                    await connection.DisposeAsync();
            }
        }

        protected async Task<T?> QuerySingleAsync(
            string query,
            List<QueryParameter> parameters,
            Func<SqlDataReader, T> map)
        {
            List<T> results = await QueryAsync(query, parameters, map);
            return results.FirstOrDefault();
        }

        protected Task<List<T>> QueryAsync(
            string query,
            List<QueryParameter> parameters,
            Func<SqlDataReader, T> map)
        {
            return RunAsync(query, parameters, async command =>
            {
                var result = new List<T>();
                await using SqlDataReader reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    result.Add(map(reader));
                }
                return result;   // the reader is disposed when this lambda ends
            });
        }

        protected Task<int> ExecuteAsync(string query, List<QueryParameter> parameters)
        {
            // returns the number of rows affected
            return RunAsync(query, parameters, command => command.ExecuteNonQueryAsync());
        }

        protected async Task<TResult> ExecuteScalarAsync<TResult>(
            string query,
            List<QueryParameter> parameters)
        {
            object? result = await RunAsync(query, parameters, command => command.ExecuteScalarAsync());

            if (result == null || result == DBNull.Value)
                throw new InvalidOperationException("The query did not return a value.");

            // Direct cast on purpose: a wrong type fails loudly (InvalidCastException).
            return (TResult)result;
        }
    }
}       