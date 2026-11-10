using Inventory.DataAccess.General_abstract;
using Inventory.DataAccess.UnitOfWork;
using Inventory.Domain.Contacts;
using Inventory.Domain.Orders.Invoices;
using Inventory.Domain.Repository_Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Inventory.DataAccess.Repositories;

public class SqlPurchaseInvoiceRepository
    : SqlRepositoryBase<PurchaseInvoice>, IPurchaseInvoiceRepository
{
    public SqlPurchaseInvoiceRepository(SqlUnitOfWork unitOfWork)
: base(unitOfWork)
    {
    }

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

    public async Task<IReadOnlyList<PurchaseInvoice>> FindBySupplierNameAsync(string name)
        => await FindBySupplierColumnAsync("s.Name", name);

    public async Task<IReadOnlyList<PurchaseInvoice>> FindBySupplierAddressAsync(string address)
        => await FindBySupplierColumnAsync("s.Address", address);

    public async Task<IReadOnlyList<PurchaseInvoice>> FindBySupplierEmailAsync(string email)
        => await FindBySupplierColumnAsync("s.Email", email);

    public async Task<IReadOnlyList<PurchaseInvoice>> FindBySupplierPhoneNumberAsync(string phoneNumber)
        => await FindBySupplierColumnAsync("s.PhoneNumber", phoneNumber);

    private async Task<IReadOnlyList<PurchaseInvoice>> FindBySupplierColumnAsync(
        string column, string value)
    {
        string query = $"{SelectQuery} WHERE {column} = @Value";

        return await QueryAsync(
            query,
            new() { new("@Value", SqlDbType.NVarChar, value) },
            BuildTable);
    }

    public async Task<IReadOnlyList<PurchaseInvoice>> GetAllInvoicesBySupplierIDAsync(
        int supplierID)
    {
        return await QueryAsync(
            SelectQuery + " WHERE pi.SupplierID = @SupplierID",
            new() { new("@SupplierID", SqlDbType.Int, supplierID) },
            BuildTable);
    }

    public async Task<IReadOnlyList<PurchaseInvoice>> GetAllInvoicesByDateAsync(
        DateTime dateTime)
    {
        return await QueryAsync(
            SelectQuery + " WHERE CAST(pi.InvoiceDate AS date) = @Date",
            new() { new("@Date", SqlDbType.Date, dateTime.Date) },
            BuildTable);
    }
}