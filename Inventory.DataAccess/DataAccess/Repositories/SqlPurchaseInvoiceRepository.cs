using Inventory.Contacts;
using Inventory.DataAccess.Database;
using Inventory.General_Interfaces_and_abstract.General_abstract;
using Inventory.General_Interfaces_and_abstract.Repository_Interfaces;
using Inventory.Orders.Invoices;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Inventory.DataAccess.Repositories;

public class SqlPurchaseInvoiceRepository
    : SqlRepositoryBase<PurchaseInvoice>, IPurchaseInvoiceRepository
{
    private const string SelectQuery = @"
        SELECT
            pi.PurchaseInvoiceID,
            pi.SupplierID,
            pi.InvoiceDate,
            pi.TotalAmount,
            pi.PaidStatus,
            s.Name AS SupplierName,
            s.Address,
            s.Email,
            s.PhoneNumber
        FROM PurchaseInvoices pi
        INNER JOIN Suppliers s ON pi.SupplierID = s.SupplierID";

    protected override PurchaseInvoice BuildTable(SqlDataReader reader)
    {
        Supplier supplier = new(
            (string)reader["SupplierName"],
            reader["Address"] == DBNull.Value ? "" : (string)reader["Address"],
            reader["Email"] == DBNull.Value ? "" : (string)reader["Email"],
            reader["PhoneNumber"] == DBNull.Value ? "" : (string)reader["PhoneNumber"]);

        supplier.SetId((int)reader["SupplierID"]);

        PurchaseInvoice invoice = new(
            supplier,
            (DateTime)reader["InvoiceDate"]);

        invoice.SetId((int)reader["PurchaseInvoiceID"]);
        invoice.SetStatus((PurchaseInvoice.Status)(byte)reader["PaidStatus"]);

        return invoice;
    }

    public async Task<int> AddAsync(PurchaseInvoice item)
    {
        ArgumentNullException.ThrowIfNull(item);

        const string query = @"
            INSERT INTO PurchaseInvoices
                (SupplierID, InvoiceDate, TotalAmount, PaidStatus)
            OUTPUT INSERTED.PurchaseInvoiceID
            VALUES
                (@SupplierID, @InvoiceDate, @TotalAmount, @PaidStatus);";
        List<QueryParameter> parameters = new()
        {
            new("@SupplierID", SqlDbType.Int, item.Contact.Id),
            new("@InvoiceDate", SqlDbType.DateTime, item.InvoiceDate),
            new("@TotalAmount", SqlDbType.Decimal, item.InvoiceAmount),
            new("@PaidStatus", SqlDbType.TinyInt, (byte)item.CurrentStatus)
        };
        int id = await ExecuteScalarAsync<int>(query, parameters);
        item.SetId(id);
        return id;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string query =
            "DELETE FROM PurchaseInvoices WHERE PurchaseInvoiceID = @Id";

        return await ExecuteAsync(query, new()
        {
            new("@Id", SqlDbType.Int, id)
        }) > 0;
    }

    public async Task<IReadOnlyList<PurchaseInvoice>> GetAllAsync()
    {
        return await QueryAsync(SelectQuery, new(), BuildTable);
    }

    public async Task<PurchaseInvoice?> GetByIdAsync(int id)
    {
        return await QuerySingleAsync(
            SelectQuery + " WHERE pi.PurchaseInvoiceID = @Id",
            new() { new("@Id", SqlDbType.Int, id) },
            BuildTable);
    }

    public Task<IReadOnlyList<PurchaseInvoice>> FindBySupplierName(string name)
        => FindBySupplierColumn("s.Name", name);

    public Task<IReadOnlyList<PurchaseInvoice>> FindBySupplierAddress(string address)
        => FindBySupplierColumn("s.Address", address);

    public Task<IReadOnlyList<PurchaseInvoice>> FindBySupplierEmail(string email)
        => FindBySupplierColumn("s.Email", email);

    public Task<IReadOnlyList<PurchaseInvoice>> FindBySupplierPhoneNumber(string phoneNumber)
        => FindBySupplierColumn("s.PhoneNumber", phoneNumber);

    private async Task<IReadOnlyList<PurchaseInvoice>> FindBySupplierColumn(
        string column, string value)
    {
        string query = $"{SelectQuery} WHERE {column} = @Value";

        return await QueryAsync(
            query,
            new() { new("@Value", SqlDbType.NVarChar, value) },
            BuildTable);
    }

    public async Task<IReadOnlyList<PurchaseInvoice>> GetAllInvoicesBySupplierID(
        int supplierID)
    {
        return await QueryAsync(
            SelectQuery + " WHERE pi.SupplierID = @SupplierID",
            new() { new("@SupplierID", SqlDbType.Int, supplierID) },
            BuildTable);
    }

    public async Task<IReadOnlyList<PurchaseInvoice>> GetAllInvoicesByDate(
        DateTime dateTime)
    {
        return await QueryAsync(
            SelectQuery + " WHERE CAST(pi.InvoiceDate AS date) = @Date",
            new() { new("@Date", SqlDbType.Date, dateTime.Date) },
            BuildTable);
    }
}