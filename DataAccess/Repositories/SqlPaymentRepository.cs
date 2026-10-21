using Inventory.Contacts;
using Inventory.DataAccess.Database;
using Inventory.General_Interfaces_and_abstract.General_abstract;
using Inventory.General_Interfaces_and_abstract.Repository_Interfaces;
using Inventory.Orders.Invoices;
using Inventory.Orders.Invoices.Interfaces;
using Inventory.Payements.Abstract;
using Inventory.Payements.Types;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Inventory.DataAccess.Repositories;

internal class SqlPaymentRepository
    : SqlRepositoryBase<Payment>, IPaymentRepository
{
    private const string SelectQuery = @"
        SELECT
            pay.PaymentID,
            pay.Amount,
            pay.PaymentDate,
            pay.Last4digits,
            pay.PaymentStatus,
            pt.PaymentTypeName,
            pay.PurchaseInvoiceID,
            pay.SalesInvoiceID,

            si.ClientID,
            si.InvoiceDate AS SalesInvoiceDate,
            si.PaidStatus AS SalesInvoiceStatus,
            c.Name AS ClientName,
            c.Address AS ClientAddress,
            c.Email AS ClientEmail,
            c.PhoneNumber AS ClientPhoneNumber,
            c.PromotionAccept,

            pi.SupplierID,
            pi.InvoiceDate AS PurchaseInvoiceDate,
            pi.PaidStatus AS PurchaseInvoiceStatus,
            s.Name AS SupplierName,
            s.Address AS SupplierAddress,
            s.Email AS SupplierEmail,
            s.PhoneNumber AS SupplierPhoneNumber

        FROM Payments pay
        INNER JOIN PaymentTypes pt
            ON pay.PaymentTypeID = pt.PaymentTypeID
        LEFT JOIN SalesInvoices si
            ON pay.SalesInvoiceID = si.SalesInvoiceID
        LEFT JOIN Clients c
            ON si.ClientID = c.ClientID
        LEFT JOIN PurchaseInvoices pi
            ON pay.PurchaseInvoiceID = pi.PurchaseInvoiceID
        LEFT JOIN Suppliers s
            ON pi.SupplierID = s.SupplierID";

    protected override Payment BuildTable(SqlDataReader reader)
    {
        IInvoice invoice;

        if (reader["SalesInvoiceID"] != DBNull.Value)
        {
            Client client = new(
                reader["PromotionAccept"] != DBNull.Value &&
                    (bool)reader["PromotionAccept"],
                (string)reader["ClientName"],
                reader["ClientAddress"] == DBNull.Value ? "" : (string)reader["ClientAddress"],
                reader["ClientEmail"] == DBNull.Value ? "" : (string)reader["ClientEmail"],
                reader["ClientPhoneNumber"] == DBNull.Value ? "" : (string)reader["ClientPhoneNumber"]);

            client.SetId((int)reader["ClientID"]);

            SalesInvoice salesInvoice = new(
                client,
                (DateTime)reader["SalesInvoiceDate"]);

            salesInvoice.SetId((int)reader["SalesInvoiceID"]);
            salesInvoice.SetStatus(
                (SalesInvoice.Status)(byte)reader["SalesInvoiceStatus"]);

            invoice = salesInvoice;
        }
        else if (reader["PurchaseInvoiceID"] != DBNull.Value)
        {
            Supplier supplier = new(
                (string)reader["SupplierName"],
                reader["SupplierAddress"] == DBNull.Value ? "" : (string)reader["SupplierAddress"],
                reader["SupplierEmail"] == DBNull.Value ? "" : (string)reader["SupplierEmail"],
                reader["SupplierPhoneNumber"] == DBNull.Value ? "" : (string)reader["SupplierPhoneNumber"]);

            supplier.SetId((int)reader["SupplierID"]);

            PurchaseInvoice purchaseInvoice = new(
                supplier,
                (DateTime)reader["PurchaseInvoiceDate"]);

            purchaseInvoice.SetId((int)reader["PurchaseInvoiceID"]);
            purchaseInvoice.SetStatus(
                (PurchaseInvoice.Status)(byte)reader["PurchaseInvoiceStatus"]);

            invoice = purchaseInvoice;
        }
        else
        {
            throw new InvalidOperationException(
                "This payment is not linked to an invoice.");
        }

        decimal amount = (decimal)reader["Amount"];
        DateTime date = (DateTime)reader["PaymentDate"];
        string typeName = (string)reader["PaymentTypeName"];

        Payment payment = typeName.Trim().ToLowerInvariant() switch
        {
            "cash" or "cashpayment" =>
                new CashPayment(invoice, amount, date),

            "card" or "cardpayment" =>
                new CardPayment(
                    reader["Last4digits"] == DBNull.Value
                        ? throw new InvalidOperationException(
                            "A card payment has no last-four-digits value.")
                        : (string)reader["Last4digits"],
                    invoice,
                    amount,
                    date),

            "bank transfer" or "banktransfer" or "banktransferpayment" =>
                new BankTransferPayment(invoice, amount, date),

            "crypto" or "cryptopayment" =>
                new CryptoPayment(invoice, amount, date),

            _ => throw new InvalidOperationException(
                $"Unknown payment type: {typeName}")
        };

        payment.SetId((int)reader["PaymentID"]);
        payment.SetStatus((Payment.PaymentStatus)(byte)reader["PaymentStatus"]);

        return payment;
    }

    public async Task AddAsync(Payment item)
    {
        ArgumentNullException.ThrowIfNull(item);

        int? salesInvoiceId = null;
        int? purchaseInvoiceId = null;

        if (item.Invoice is SalesInvoice)
        {
            salesInvoiceId = item.Invoice.InvoiceId;
        }
        else if (item.Invoice is PurchaseInvoice)
        {
            purchaseInvoiceId = item.Invoice.InvoiceId;
        }
        else
        {
            throw new ArgumentException(
                "The payment must belong to a sales or purchase invoice.",
                nameof(item));
        }

        string paymentTypeName = item switch
        {
            CashPayment => "Cash",
            CardPayment => "Card",
            BankTransferPayment => "Bank Transfer",
            CryptoPayment => "Crypto",
            _ => throw new ArgumentException(
                "Unsupported payment type.", nameof(item))
        };

        const string query = @"
            DECLARE @TypeID int =
            (
                SELECT PaymentTypeID
                FROM PaymentTypes
                WHERE PaymentTypeName = @TypeName
            );

            IF @TypeID IS NULL
                THROW 50001, 'Payment type not found in PaymentTypes.', 1;

            INSERT INTO Payments
            (
                PurchaseInvoiceID,
                SalesInvoiceID,
                Amount,
                PaymentDate,
                Last4digits,
                PaymentTypeID,
                PaymentStatus
            )
            OUTPUT INSERTED.PaymentID
            VALUES
            (
                @PurchaseInvoiceID,
                @SalesInvoiceID,
                @Amount,
                @PaymentDate,
                @Last4digits,
                @TypeID,
                @PaymentStatus
            );";

        using SqlConnection connection = SqlConnectionFactory.CreateConnection();
        using SqlCommand command = new(query, connection);

        command.Parameters.Add("@PurchaseInvoiceID", SqlDbType.Int).Value =
            (object?)purchaseInvoiceId ?? DBNull.Value;

        command.Parameters.Add("@SalesInvoiceID", SqlDbType.Int).Value =
            (object?)salesInvoiceId ?? DBNull.Value;

        SqlParameter amountParameter =
            command.Parameters.Add("@Amount", SqlDbType.Decimal);
        amountParameter.Precision = 10;
        amountParameter.Scale = 2;
        amountParameter.Value = item.Amount;

        command.Parameters.Add("@PaymentDate", SqlDbType.DateTime2).Value =
            item.Date;

        command.Parameters.Add("@Last4digits", SqlDbType.NVarChar, 20).Value =
            item is CardPayment card
                ? card.Last4digits
                : DBNull.Value;

        command.Parameters.Add("@TypeName", SqlDbType.NVarChar, 50).Value =
            paymentTypeName;

        command.Parameters.Add("@PaymentStatus", SqlDbType.TinyInt).Value =
            (byte)item.Status;

        await connection.OpenAsync();

        int id = (int)(await command.ExecuteScalarAsync())!;
        item.SetId(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string query =
            "DELETE FROM Payments WHERE PaymentID = @Id";

        return await ExecuteAsync(query, new()
        {
            new("@Id", SqlDbType.Int, id)
        }) > 0;
    }

    public async Task<IReadOnlyList<Payment>> GetAllAsync()
    {
        return await QueryAsync(SelectQuery, new(), BuildTable);
    }

    public async Task<Payment?> GetByIdAsync(int id)
    {
        return await QuerySingleAsync(
            SelectQuery + " WHERE pay.PaymentID = @Id",
            new() { new("@Id", SqlDbType.Int, id) },
            BuildTable);
    }

    public async Task<IReadOnlyList<Payment>> GetByInvoiceIdAsync(int invoiceId)
    {
        return await QueryAsync(
            SelectQuery + @"
                WHERE pay.SalesInvoiceID = @InvoiceID
                   OR pay.PurchaseInvoiceID = @InvoiceID",
            new() { new("@InvoiceID", SqlDbType.Int, invoiceId) },
            BuildTable);
    }
}