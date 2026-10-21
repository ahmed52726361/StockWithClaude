
using Inventory.Contacts;
using Inventory.DataAccess.Database;
using Inventory.General_Interfaces_and_abstract.General_abstract;
using Inventory.General_Interfaces_and_abstract.Repository_Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Inventory.DataAccess.Repositories
{
    internal class SqlSupplierRepository
        : SqlRepositoryBase<Supplier>, ISupplierRepository
    {
        private const string SelectQuery = @"
            SELECT SupplierID, Name, Address, Email, PhoneNumber
            FROM Suppliers";

        protected override Supplier BuildTable(SqlDataReader reader)
        {
            Supplier supplier = new Supplier(
                (string)reader["Name"],
                reader["Address"] as string ?? string.Empty,
                reader["Email"] as string ?? string.Empty,
                reader["PhoneNumber"] as string ?? string.Empty);

            supplier.SetId((int)reader["SupplierID"]);

            return supplier;
        }

        public async Task AddAsync(Supplier item)
        {
            ArgumentNullException.ThrowIfNull(item);

            const string query = @"
                INSERT INTO Suppliers
                    (Name, Address, Email, PhoneNumber)
                VALUES
                    (@Name, @Address, @Email, @PhoneNumber);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            List<QueryParameter> parameters = new()
            {
                new("@Name", SqlDbType.NVarChar, item.Name),
                new("@Address", SqlDbType.NVarChar, item.Address),
                new("@Email", SqlDbType.NVarChar, item.Email),
                new("@PhoneNumber", SqlDbType.NVarChar, item.PhoneNumber)
            };

            using SqlConnection connection =
                SqlConnectionFactory.CreateConnection();
            using SqlCommand command = new(query, connection);

            foreach (QueryParameter parameter in parameters)
            {
                command.Parameters.Add(parameter.Name, parameter.Type)
                    .Value = parameter.Value ?? DBNull.Value;
            }

            await connection.OpenAsync();

            int id = Convert.ToInt32(
                await command.ExecuteScalarAsync());

            item.SetId(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(id, 0);

            const string query =
                "DELETE FROM Suppliers WHERE SupplierID = @Id";

            List<QueryParameter> parameters = new()
            {
                new("@Id", SqlDbType.Int, id)
            };

            return await ExecuteAsync(query, parameters) > 0;
        }

        public Task<Supplier?> FindByName(string name) =>
            FindOneAsync("Name", name);

        public Task<Supplier?> FindByAddress(string address) =>
            FindOneAsync("Address", address);

        public Task<Supplier?> FindByEmail(string email) =>
            FindOneAsync("Email", email);

        public Task<Supplier?> FindByPhoneNumber(string phoneNumber) =>
            FindOneAsync("PhoneNumber", phoneNumber);

        private Task<Supplier?> FindOneAsync(string column, string value)
        {
            ArgumentNullException.ThrowIfNull(value);

            string query = SelectQuery + $" WHERE {column} = @Value";

            List<QueryParameter> parameters = new()
            {
                new("@Value", SqlDbType.NVarChar, value)
            };

            return QuerySingleAsync(query, parameters, BuildTable);
        }

        public async Task<IReadOnlyList<Supplier>> GetAllAsync()
        {
            return await QueryAsync(
                SelectQuery,
                new List<QueryParameter>(),
                BuildTable);
        }

        public async Task<Supplier?> GetByIdAsync(int id)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(id, 0);

            string query = SelectQuery + " WHERE SupplierID = @Id";

            List<QueryParameter> parameters = new()
            {
                new("@Id", SqlDbType.Int, id)
            };

            return await QuerySingleAsync(query, parameters, BuildTable);
        }
    }
}
