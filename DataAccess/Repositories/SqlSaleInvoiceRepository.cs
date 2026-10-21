using Inventory.Contacts;
using Inventory.DataAccess.Database;
using Inventory.General_Interfaces_and_abstract.General_abstract;
using Inventory.General_Interfaces_and_abstract.Repository_Interfaces;
using Inventory.Orders.Invoices;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Inventory.DataAccess.Repositories;

internal class SqlSaleInvoiceRepository
    : SqlRepositoryBase<SalesInvoice>, ISaleInvoiceRepository
{
    private const string SelectQuery = @"
        SELECT
            si.SalesInvoiceID,
            si.ClientID,
            si.InvoiceDate,
            si.TotalAmount,
            si.PaidStatus,
            c.Name AS ClientName,
            c.Address,
            c.Email,
            c.PhoneNumber,
            c.PromotionAccept
        FROM SalesInvoices si
        INNER JOIN Clients c ON si.ClientID = c.ClientID";

    protected override SalesInvoice BuildTable(SqlDataReader reader)
    {
        Client client = new(
            reader["PromotionAccept"] != DBNull.Value &&
                (bool)reader["PromotionAccept"],
            (string)reader["ClientName"],
            reader["Address"] == DBNull.Value ? "" : (string)reader["Address"],
            reader["Email"] == DBNull.Value ? "" : (string)reader["Email"],
            reader["PhoneNumber"] == DBNull.Value ? "" : (string)reader["PhoneNumber"]);

        client.SetId((int)reader["ClientID"]);

        SalesInvoice invoice = new(
            client,
            (DateTime)reader["InvoiceDate"]);

        invoice.SetId((int)reader["SalesInvoiceID"]);
        invoice.SetStatus((SalesInvoice.Status)(byte)reader["PaidStatus"]);

        return invoice;
    }

    public async Task AddAsync(SalesInvoice item)
    {
        ArgumentNullException.ThrowIfNull(item);

        const string query = @"
            INSERT INTO SalesInvoices
                (ClientID, InvoiceDate, TotalAmount, PaidStatus)
            OUTPUT INSERTED.SalesInvoiceID
            VALUES
                (@ClientID, @InvoiceDate, @TotalAmount, @PaidStatus);";

        using SqlConnection connection = SqlConnectionFactory.CreateConnection();
        using SqlCommand command = new(query, connection);

        command.Parameters.Add("@ClientID", SqlDbType.Int).Value =
            item.Contact.Id;

        command.Parameters.Add("@InvoiceDate", SqlDbType.DateTime2).Value =
            item.InvoiceDate;

        SqlParameter amount = command.Parameters.Add(
            "@TotalAmount", SqlDbType.Decimal);
        amount.Precision = 10;
        amount.Scale = 2;
        amount.Value = item.InvoiceAmount;

        command.Parameters.Add("@PaidStatus", SqlDbType.TinyInt).Value =
            (byte)item.CurrentStatus;

        await connection.OpenAsync();

        int id = (int)(await command.ExecuteScalarAsync())!;
        item.SetId(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string query =
            "DELETE FROM SalesInvoices WHERE SalesInvoiceID = @Id";

        return await ExecuteAsync(query, new()
        {
            new("@Id", SqlDbType.Int, id)
        }) > 0;
    }

    public async Task<IReadOnlyList<SalesInvoice>> GetAllAsync()
    {
        return await QueryAsync(SelectQuery, new(), BuildTable);
    }

    public async Task<SalesInvoice?> GetByIdAsync(int id)
    {
        return await QuerySingleAsync(
            SelectQuery + " WHERE si.SalesInvoiceID = @Id",
            new() { new("@Id", SqlDbType.Int, id) },
            BuildTable);
    }

    public Task<IReadOnlyList<SalesInvoice>> FindByClientName(string name)
        => FindByClientColumn("c.Name", name);

    public Task<IReadOnlyList<SalesInvoice>> FindByClientAddress(string address)
        => FindByClientColumn("c.Address", address);

    public Task<IReadOnlyList<SalesInvoice>> FindByClientEmail(string email)
        => FindByClientColumn("c.Email", email);

    public Task<IReadOnlyList<SalesInvoice>> FindByClientPhoneNumber(string phoneNumber)
        => FindByClientColumn("c.PhoneNumber", phoneNumber);

    private Task<IReadOnlyList<SalesInvoice>> FindByClientColumn(
        string column, string value)
    {
        // Column names are fixed by the methods above, never supplied by user input.
        string query = $"{SelectQuery} WHERE {column} = @Value";

        return QueryAsync(
            query,
            new() { new("@Value", SqlDbType.NVarChar, value) },
            BuildTable)
            .ContinueWith<IReadOnlyList<SalesInvoice>>(
                task => task.GetAwaiter().GetResult());
    }

    public async Task<IReadOnlyList<SalesInvoice>> GetAllInvoicesByClientID(
        int clientId)
    {
        return await QueryAsync(
            SelectQuery + " WHERE si.ClientID = @ClientID",
            new() { new("@ClientID", SqlDbType.Int, clientId) },
            BuildTable);
    }

    public async Task<IReadOnlyList<SalesInvoice>> GetAllInvoicesByDate(
        DateTime dateTime)
    {
        return await QueryAsync(
            SelectQuery + " WHERE CAST(si.InvoiceDate AS date) = @Date",
            new() { new("@Date", SqlDbType.Date, dateTime.Date) },
            BuildTable);
    }
}