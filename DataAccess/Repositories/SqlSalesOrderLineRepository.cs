using Inventory.Contacts;
using Inventory.General_Interfaces_and_abstract.General_abstract;
using Inventory.General_Interfaces_and_abstract.Repository_Interfaces;
using Inventory.Orders.Invoices;
using Inventory.Orders.Lines;
using Inventory.Products;
using Microsoft.Data.SqlClient;
using System.Data;

internal class SqlSalesOrderLineRepository
    : SqlRepositoryBase<SalesOrderLine>, ISalesOrderLineRepository
{

    public async Task AddAsync(SalesOrderLine item)
    {
        ArgumentNullException.ThrowIfNull(item);

        string query = @"
        INSERT INTO SalesOrderLines
        (
            SalesInvoiceID,
            ProductID,
            OrderQty,
            OrderPrice
        )
        VALUES
        (
            @SalesInvoiceID,
            @ProductID,
            @OrderQty,
            @OrderPrice
        )";

        List<QueryParameter> parameters = new()
        {
            new("@SalesInvoiceID", SqlDbType.Int, item.Invoice.Id),
            new("@ProductID", SqlDbType.Int, item.Product.Id),
            new("@OrderQty", SqlDbType.Int, item.Qty),
            new("@OrderPrice", SqlDbType.Decimal, item.OrderPrice)
        };

        await ExecuteAsync(query, parameters);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(id, 0);

        string query =
            @"DELETE FROM SalesOrderLines WHERE SalesOrderLineID = @id";

        List<QueryParameter> parameters = new()
        {
            new("@id", SqlDbType.Int, id)
        };

        return await ExecuteAsync(query, parameters) > 0;
    }

    public async Task<IReadOnlyList<SalesOrderLine>> FindByInvoiceID(int invoiceID)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(invoiceID, 0);

        string query = BaseSelect +
                       @" WHERE sol.SalesInvoiceID = @invoiceID";

        List<QueryParameter> parameters = new()
        {
            new("@invoiceID", SqlDbType.Int, invoiceID)
        };

        return await QueryAsync(query, parameters, BuildTable);
    }

    public async Task<IReadOnlyList<SalesOrderLine>> FindByProductID(int productID)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(productID, 0);

        string query = BaseSelect +
                       @" WHERE sol.ProductID = @productID";

        List<QueryParameter> parameters = new()
        {
            new("@productID", SqlDbType.Int, productID)
        };

        return await QueryAsync(query, parameters, BuildTable);
    }

    public async Task<IReadOnlyList<SalesOrderLine>> GetAllAsync()
    {
        return await QueryAsync(
            BaseSelect,
            new List<QueryParameter>(),
            BuildTable);
    }

    public async Task<SalesOrderLine?> GetByIdAsync(int id)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(id, 0);

        string query = BaseSelect +
                       @" WHERE sol.SalesOrderLineID = @id";

        List<QueryParameter> parameters = new()
        {
            new("@id", SqlDbType.Int, id)
        };

        return await QuerySingleAsync(query, parameters, BuildTable);
    }

    public async Task<IReadOnlyList<SalesOrderLine>> GetAllOrdersPriceGreaterThan(decimal price)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(price, 0);

        string query = BaseSelect +
                       @" WHERE sol.OrderPrice > @price";

        List<QueryParameter> parameters = new()
        {
            new("@price", SqlDbType.Decimal, price)
        };

        return await QueryAsync(query, parameters, BuildTable);
    }

    public async Task<IReadOnlyList<SalesOrderLine>> GetAllOrdersPriceLessThan(decimal price)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(price, 0);

        string query = BaseSelect +
                       @" WHERE sol.OrderPrice < @price";

        List<QueryParameter> parameters = new()
        {
            new("@price", SqlDbType.Decimal, price)
        };

        return await QueryAsync(query, parameters, BuildTable);
    }

    public async Task<IReadOnlyList<SalesOrderLine>> GetAllOrdersQtyGreaterThan(int qty)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(qty, 0);

        string query = BaseSelect +
                       @" WHERE sol.OrderQty > @qty";

        List<QueryParameter> parameters = new()
        {
            new("@qty", SqlDbType.Int, qty)
        };

        return await QueryAsync(query, parameters, BuildTable);
    }

    public async Task<IReadOnlyList<SalesOrderLine>> GetAllOrdersQtyLessThan(int qty)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(qty, 0);

        string query = BaseSelect +
                       @" WHERE sol.OrderQty < @qty";

        List<QueryParameter> parameters = new()
        {
            new("@qty", SqlDbType.Int, qty)
        };

        return await QueryAsync(query, parameters, BuildTable);
    }

    protected override SalesOrderLine BuildTable(SqlDataReader reader)
    {
        Product product = new(
            (string)reader["ProductName"],
            (decimal)reader["Price"],
            (int)reader["StockQty"]);

        product.SetId((int)reader["ProductID"]);

        Client client = new(
            (bool)reader["PromotionAccept"],
            (string)reader["ClientName"],
            (string)reader["Address"],
            (string)reader["Email"],
            (string)reader["PhoneNumber"]);

        client.SetId((int)reader["ClientID"]);

        SalesInvoice invoice = new(client, (DateTime)reader["InvoiceDate"]);
        invoice.SetId((int)reader["SalesInvoiceID"]);

        byte statusValue = (byte)reader["PaidStatus"];
        invoice.SetStatus((SalesInvoice.Status)statusValue);
        
        SalesOrderLine line = new(
            invoice,
            product,
            (int)reader["OrderQty"],
            (decimal)reader["OrderPrice"]);

        line.SetId((int)reader["SalesOrderLineID"]);

        return line;
    }
}