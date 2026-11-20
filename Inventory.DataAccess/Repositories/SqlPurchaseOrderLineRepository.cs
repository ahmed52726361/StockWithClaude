using Inventory.DataAccess.Database;
using Inventory.DataAccess.General_abstract;
using Inventory.DataAccess.UnitOfWork;
using Inventory.Domain.Contacts;
using Inventory.Domain.Orders.Invoices;
using Inventory.Domain.Orders.Lines;
using Inventory.Domain.Products;
using Inventory.Domain.Repository_Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Inventory.DataAccess.Repositories;

public class SqlPurchaseOrderLineRepository
    : SqlRepositoryBase<PurchaseOrderLine>, IPurchaseOrderLineRepository
{
    public SqlPurchaseOrderLineRepository(SqlUnitOfWork unitOfWork)
: base(unitOfWork)
    {
    }

    private const string SelectQuery = @"
        SELECT
            pol.PurchaseOrderLineID,
            pol.OrderQty,
            pol.OrderPrice,

            pi.PurchaseInvoiceID,
            pi.InvoiceDate,
            pi.PaidStatus,

            s.SupplierID,
            s.Name AS SupplierName,
            s.Address,
            s.Email,
            s.PhoneNumber,

            p.ProductID,
            p.Name AS ProductName,
            p.StockQty,
            p.Price
        FROM PurchaseOrderLines pol
        INNER JOIN PurchaseInvoices pi
            ON pol.PurchaseInvoiceID = pi.PurchaseInvoiceID
        INNER JOIN Suppliers s
            ON pi.SupplierID = s.SupplierID
        INNER JOIN Products p
            ON pol.ProductID = p.ProductID";

    protected override PurchaseOrderLine BuildTable(SqlDataReader reader)
    {
        Product product = new(
            (string)reader["ProductName"],
            (decimal)reader["Price"],
            (int)reader["StockQty"]);

        product.SetId((int)reader["ProductID"]);

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

        PurchaseOrderLine line = new(
            invoice,
            product,
            (int)reader["OrderQty"],
            (decimal)reader["OrderPrice"]);

        line.SetId((int)reader["PurchaseOrderLineID"]);

        return line;
    }


    // this method is so hard and i don't know if  I need to learn it as alone 
    public async Task<int> AddAsync(PurchaseOrderLine item)
    {
        ArgumentNullException.ThrowIfNull(item);

        const string insertQuery = @"
        INSERT INTO PurchaseOrderLines
        (
            PurchaseInvoiceID,
            ProductID,
            OrderQty,
            OrderPrice
        )
        OUTPUT INSERTED.PurchaseOrderLineID
        VALUES
        (
            @InvoiceID,
            @ProductID,
            @Qty,
            @Price
        );";

        List<QueryParameter> parameters = new()
         {
             new("@InvoiceID", SqlDbType.Int, item.Invoice.Id),
            new("@ProductID", SqlDbType.Int, item.Product.Id),
            new("@Qty", SqlDbType.Int, item.Qty),
            new("@Price", SqlDbType.Decimal, item.OrderPrice)
         };
        int id = await ExecuteScalarAsync<int>(insertQuery, parameters);
        item.SetId(id);
        return id;


    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string query =
            "DELETE FROM PurchaseOrderLines WHERE PurchaseOrderLineID = @Id";

        return await ExecuteAsync(query, new()
        {
            new("@Id", SqlDbType.Int, id)
        }) > 0;
    }

    public async Task<IReadOnlyList<PurchaseOrderLine>> GetAllAsync()
    {
        return await QueryAsync(SelectQuery, new(), BuildTable);
    }

    public async Task<PurchaseOrderLine?> GetByIdAsync(int id)
    {
        return await QuerySingleAsync(
            SelectQuery + " WHERE pol.PurchaseOrderLineID = @Id",
            new() { new("@Id", SqlDbType.Int, id) },
            BuildTable);
    }

    public async Task<IReadOnlyList<PurchaseOrderLine>> FindByInvoiceIDAsync(
        int invoiceID)
    {
        return await QueryAsync(
            SelectQuery + " WHERE pol.PurchaseInvoiceID = @InvoiceID",
            new() { new("@InvoiceID", SqlDbType.Int, invoiceID) },
            BuildTable);
    }

    public async Task<IReadOnlyList<PurchaseOrderLine>> FindByProductIDAsync(
        int productID)
    {
        return await QueryAsync(
            SelectQuery + " WHERE pol.ProductID = @ProductID",
            new() { new("@ProductID", SqlDbType.Int, productID) },
            BuildTable);
    }

    public async Task<IReadOnlyList<PurchaseOrderLine>> GetAllOrdersQtyLessThanAsync(
        int qty)
    {
        return await QueryAsync(
            SelectQuery + " WHERE pol.OrderQty < @Qty",
            new() { new("@Qty", SqlDbType.Int, qty) },
            BuildTable);
    }

    public async Task<IReadOnlyList<PurchaseOrderLine>> GetAllOrdersQtyGreaterThanAsync(
        int qty)
    {
        return await QueryAsync(
            SelectQuery + " WHERE pol.OrderQty > @Qty",
            new() { new("@Qty", SqlDbType.Int, qty) },
            BuildTable);
    }

    public async Task<IReadOnlyList<PurchaseOrderLine>> GetAllOrdersPriceLessThanAsync(
        decimal price)
    {
        return await QueryAsync(
            SelectQuery + " WHERE pol.OrderPrice < @Price",
            new() { new("@Price", SqlDbType.Decimal, price) },
            BuildTable);
    }

    public async Task<IReadOnlyList<PurchaseOrderLine>> GetAllOrdersPriceGreaterThanAsync(
        decimal price)
    {
        return await QueryAsync(
            SelectQuery + " WHERE pol.OrderPrice > @Price",
            new() { new("@Price", SqlDbType.Decimal, price) },
            BuildTable);
    }
}