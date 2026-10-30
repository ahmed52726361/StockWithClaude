using Inventory.Contacts;
using Inventory.DataAccess.Database;
using Inventory.General_Interfaces_and_abstract.General_abstract;
using Inventory.General_Interfaces_and_abstract.Repository_Interfaces;
using Inventory.Orders.Invoices;
using Inventory.Orders.Lines;
using Inventory.Products;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Inventory.DataAccess.Repositories;

public class SqlPurchaseOrderLineRepository
    : SqlRepositoryBase<PurchaseOrderLine>, IPurchaseOrderLineRepository
{
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

        const string updateTotalQuery = @"
        UPDATE PurchaseInvoices
        SET TotalAmount =
        (
            SELECT COALESCE(SUM(OrderQty * OrderPrice), 0)
            FROM PurchaseOrderLines
            WHERE PurchaseInvoiceID = @InvoiceID
        )
        WHERE PurchaseInvoiceID = @InvoiceID;";

        using SqlConnection connection =
            SqlConnectionFactory.CreateConnection();

        await connection.OpenAsync();

        using SqlTransaction transaction =
            (SqlTransaction)await connection.BeginTransactionAsync();

        try
        {
            int id;

            using (SqlCommand insertCommand =
                new(insertQuery, connection, transaction))
            {
                insertCommand.Parameters.Add(
                    "@InvoiceID", SqlDbType.Int).Value = item.Invoice.Id;

                insertCommand.Parameters.Add(
                    "@ProductID", SqlDbType.Int).Value = item.Product.Id;

                insertCommand.Parameters.Add(
                    "@Qty", SqlDbType.Int).Value = item.Qty;

                SqlParameter price = insertCommand.Parameters.Add(
                    "@Price", SqlDbType.Decimal);

                price.Precision = 10;
                price.Scale = 2;
                price.Value = item.OrderPrice;

                object? result = await insertCommand.ExecuteScalarAsync();

                if (result is null || result == DBNull.Value)
                {
                    throw new InvalidOperationException(
                        "Failed to retrieve the generated purchase order line ID.");
                }

                id = Convert.ToInt32(result);
            }

            using (SqlCommand updateCommand =
                new(updateTotalQuery, connection, transaction))
            {
                updateCommand.Parameters.Add(
                    "@InvoiceID", SqlDbType.Int).Value = item.Invoice.Id;

                await updateCommand.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();

            item.SetId(id);

            return id;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
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

    public async Task<IReadOnlyList<PurchaseOrderLine>> FindByInvoiceID(
        int invoiceID)
    {
        return await QueryAsync(
            SelectQuery + " WHERE pol.PurchaseInvoiceID = @InvoiceID",
            new() { new("@InvoiceID", SqlDbType.Int, invoiceID) },
            BuildTable);
    }

    public async Task<IReadOnlyList<PurchaseOrderLine>> FindByProductID(
        int productID)
    {
        return await QueryAsync(
            SelectQuery + " WHERE pol.ProductID = @ProductID",
            new() { new("@ProductID", SqlDbType.Int, productID) },
            BuildTable);
    }

    public async Task<IReadOnlyList<PurchaseOrderLine>> GetAllOrdersQtyLessThan(
        int qty)
    {
        return await QueryAsync(
            SelectQuery + " WHERE pol.OrderQty < @Qty",
            new() { new("@Qty", SqlDbType.Int, qty) },
            BuildTable);
    }

    public async Task<IReadOnlyList<PurchaseOrderLine>> GetAllOrdersQtyGreaterThan(
        int qty)
    {
        return await QueryAsync(
            SelectQuery + " WHERE pol.OrderQty > @Qty",
            new() { new("@Qty", SqlDbType.Int, qty) },
            BuildTable);
    }

    public async Task<IReadOnlyList<PurchaseOrderLine>> GetAllOrdersPriceLessThan(
        decimal price)
    {
        return await QueryAsync(
            SelectQuery + " WHERE pol.OrderPrice < @Price",
            new() { new("@Price", SqlDbType.Decimal, price) },
            BuildTable);
    }

    public async Task<IReadOnlyList<PurchaseOrderLine>> GetAllOrdersPriceGreaterThan(
        decimal price)
    {
        return await QueryAsync(
            SelectQuery + " WHERE pol.OrderPrice > @Price",
            new() { new("@Price", SqlDbType.Decimal, price) },
            BuildTable);
    }
}