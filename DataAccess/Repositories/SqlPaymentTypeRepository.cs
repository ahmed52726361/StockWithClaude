using Inventory.DataAccess.Database;
using Inventory.General_Interfaces_and_abstract.General_abstract;
using Inventory.General_Interfaces_and_abstract.Repository_Interfaces;
using Inventory.Payements;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Inventory.DataAccess.Repositories;

internal class SqlPaymentTypeRepository
    : SqlRepositoryBase<PaymentType>, IPaymentTypeRepository
{
    private const string SelectQuery = @"
        SELECT PaymentTypeID, PaymentTypeName
        FROM PaymentTypes";

    protected override PaymentType BuildTable(SqlDataReader reader)
    {
        PaymentType type = new((string)reader["PaymentTypeName"]);
        type.SetId((int)reader["PaymentTypeID"]);
        return type;
    }

    public async Task AddAsync(PaymentType item)
    {
        ArgumentNullException.ThrowIfNull(item);

        const string query = @"
            INSERT INTO PaymentTypes (PaymentTypeName)
            OUTPUT INSERTED.PaymentTypeID
            VALUES (@Name);";

        using SqlConnection connection = SqlConnectionFactory.CreateConnection();
        using SqlCommand command = new(query, connection);

        command.Parameters.Add("@Name",SqlDbType.NVarChar,50).Value = item.PaymentTypeName;
        await connection.OpenAsync();

        int id = (int)(await command.ExecuteScalarAsync())!;
        item.SetId(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string query =
            "DELETE FROM PaymentTypes WHERE PaymentTypeID = @Id";

        return await ExecuteAsync(query, new()
        {
            new("@Id", SqlDbType.Int, id)
        }) > 0;
    }

    public async Task<IReadOnlyList<PaymentType>> GetAllAsync()
    {
        return await QueryAsync(SelectQuery, new(), BuildTable);
    }

    public async Task<PaymentType?> GetByIdAsync(int id)
    {
        return await QuerySingleAsync(
            SelectQuery + " WHERE PaymentTypeID = @Id",
            new() { new("@Id", SqlDbType.Int, id) },
            BuildTable);
    }

    public async Task<PaymentType?> FindByNameAsync(string name)
    {
        return await QuerySingleAsync(
            SelectQuery + " WHERE PaymentTypeName = @Name",
            new() { new("@Name", SqlDbType.NVarChar, name) },
            BuildTable);
    }
}