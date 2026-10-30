using Inventory.Contacts;
using Inventory.DataAccess.Database;
using Inventory.General_Interfaces_and_abstract.General_abstract;
using Inventory.General_Interfaces_and_abstract.Repository_Interfaces;
using Inventory.Orders.Invoices;
using Inventory.Orders.Lines;
using Inventory.Products;
using Microsoft.Data.SqlClient;
using System.Data;

public class SqlSalesOrderLineRepository
    : SqlRepositoryBase<SalesOrderLine>, ISalesOrderLineRepository
{

    private const string BaseSelect = @"
    SELECT
        sol.SalesOrderLineID,
        sol.OrderQty,
        sol.OrderPrice,

        si.SalesInvoiceID,
        si.InvoiceDate,
        si.TotalAmount,
        si.PaidStatus,

        c.ClientID,
        c.Name AS ClientName,
        c.Address,
        c.Email,
        c.PhoneNumber,
        c.PromotionAccept,

        p.ProductID,
        p.Name AS ProductName,
        p.StockQty,
        p.Price

    FROM SalesOrderLines sol
    JOIN SalesInvoices si ON sol.SalesInvoiceID = si.SalesInvoiceID
    JOIN Clients c ON si.ClientID = c.ClientID
    JOIN Products p ON sol.ProductID = p.ProductID";


    // I changed this method using AI and I understand it a bit but it's my first time seeing transaction so do you think we have another way or should we learn it ?

    public async Task<int> AddAsync(SalesOrderLine item)
    {
        ArgumentNullException.ThrowIfNull(item);

        const string insertQuery = @"
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
        );

        SELECT CAST(SCOPE_IDENTITY() AS int);";

        const string updateTotalQuery = @"
        UPDATE SalesInvoices
        SET TotalAmount =
        (
            SELECT COALESCE(SUM(OrderQty * OrderPrice), 0)
            FROM SalesOrderLines
            WHERE SalesInvoiceID = @SalesInvoiceID
        )
        WHERE SalesInvoiceID = @SalesInvoiceID;";

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
                    "@SalesInvoiceID", SqlDbType.Int).Value = item.Invoice.Id;

                insertCommand.Parameters.Add(
                    "@ProductID", SqlDbType.Int).Value = item.Product.Id;

                insertCommand.Parameters.Add(
                    "@OrderQty", SqlDbType.Int).Value = item.Qty;

                SqlParameter priceParameter =
                    insertCommand.Parameters.Add(
                        "@OrderPrice", SqlDbType.Decimal);

                priceParameter.Precision = 10;
                priceParameter.Scale = 2;
                priceParameter.Value = item.OrderPrice;

                object? result = await insertCommand.ExecuteScalarAsync();

                if (result is null || result == DBNull.Value)
                {
                    throw new InvalidOperationException(
                        "Failed to retrieve the generated sales order line ID.");
                }

                id = Convert.ToInt32(result);
            }

            using (SqlCommand updateCommand =
                new(updateTotalQuery, connection, transaction))
            {
                updateCommand.Parameters.Add(
                    "@SalesInvoiceID", SqlDbType.Int).Value = item.Invoice.Id;

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