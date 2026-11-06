
using Inventory.DataAccess.General_abstract;
using Inventory.Domain.Contacts;
using Inventory.Domain.Repository_Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Inventory.DataAccess.Repositories
{
    public class SqlClientRepository
        : SqlRepositoryBase<Client>, IClientRepository
    {
        protected override Client BuildTable(SqlDataReader reader)
        {
            Client client = new (
            reader["PromotionAccept"] == DBNull.Value ? (bool?)null : (bool)reader["PromotionAccept"],
            (string)reader["Name"]??string.Empty,
                reader["Address"] as string,
                reader["Email"] as string,
                reader["PhoneNumber"] as string);

            client.SetId((int)reader["ClientID"]);

            return client;
        }

        private const string SelectQuery = @"
            SELECT ClientID, Name, Address, Email, PhoneNumber,
                   PromotionAccept
            FROM Clients";

        public async Task<int> AddAsync(Client item)
        {
            ArgumentNullException.ThrowIfNull(item);

            const string query = @"
                INSERT INTO Clients
                    (Name, Address, Email, PhoneNumber, PromotionAccept)
                OUTPUT INSERTED.ClientID
                VALUES
                    (@Name, @Address, @Email, @PhoneNumber,
                 @PromotionAccept);";             
            List<QueryParameter> parameters = new()
            {
                new("@Name", SqlDbType.NVarChar, item.Name),
                new("@Address", SqlDbType.NVarChar, item.Address),
                new("@Email", SqlDbType.NVarChar, item.Email),
                new("@PhoneNumber", SqlDbType.NVarChar, item.PhoneNumber),
                new("@PromotionAccept", SqlDbType.Bit,
                    item.PromotionAccept)
            };

            // ExecuteScalarAsync is needed because this query returns the ID.
            int id =await ExecuteScalarAsync<int>(query, parameters);
            item.SetId(id);
            return id;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(id, 0);

            const string query =
                "DELETE FROM Clients WHERE ClientID = @Id";

            List<QueryParameter> parameters = new()
            {
                new("@Id", SqlDbType.Int, id)
            };

            return await ExecuteAsync(query, parameters) > 0;
        }

        public async Task<Client?> GetByIdAsync(int id)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(id, 0);

            string query = SelectQuery + " WHERE ClientID = @Id";

            List<QueryParameter> parameters = new()
            {
                new("@Id", SqlDbType.Int, id)
            };

            return await QuerySingleAsync(query, parameters, BuildTable);
        }

        public async Task<IReadOnlyList<Client>> GetAllAsync()
        {
            return await QueryAsync(
                SelectQuery,
                new List<QueryParameter>(),
                BuildTable);
        }

        public async Task<Client?> FindByNameAsync(string name)
        {
            return await FindOneAsync("Name", name);
        }

        public async Task<Client?> FindByAddressAsync(string address) =>
            await FindOneAsync("Address", address);

        public async Task<Client?> FindByEmailAsync(string email) =>
            await FindOneAsync("Email", email);

        public async Task<Client?> FindByPhoneNumberAsync(string phoneNumber) =>
            await FindOneAsync("PhoneNumber", phoneNumber);

        private async Task<Client?> FindOneAsync(string column, string value)
        {
            ArgumentNullException.ThrowIfNull(value);

            // The column comes only from the fixed method calls above.
            string query = SelectQuery + $" WHERE {column} = @Value";

            List<QueryParameter> parameters = new()
            {
                new("@Value", SqlDbType.NVarChar, value)
            };

            return await QuerySingleAsync(query, parameters, BuildTable);
        }

        public async Task<IReadOnlyList<Client>> GetByPromotionAcceptAsync(
            bool promotionAccept)
        {
            string query = SelectQuery +
                           " WHERE PromotionAccept = @PromotionAccept";

            List<QueryParameter> parameters = new()
            {
                new("@PromotionAccept", SqlDbType.Bit, promotionAccept)
            };

            return await QueryAsync(query, parameters, BuildTable);
        }
    }
}
