using Inventory.Contacts;
using Inventory.DataAccess.Database;
using Inventory.General_Interfaces_and_abstract.General_abstract;
using Inventory.General_Interfaces_and_abstract.Repository_Interfaces;
using Inventory.Orders.Invoices;
using Inventory.Orders.Invoices.Interfaces;
using Inventory.Payements;
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
            pay.DueDate,
            pay.PaidDate,   
            pay.PaymentMethod,
            pay.Last4digits,
            pay.TransferReference,
            pay.TransactionReference,
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
                reader["PromotionAccept"] == DBNull.Value
                    ? null
                    : (bool)reader["PromotionAccept"],
                (string)reader["ClientName"],
                reader["ClientAddress"] == DBNull.Value
                    ? null
                    : (string)reader["ClientAddress"],
                reader["ClientEmail"] == DBNull.Value
                    ? null
                    : (string)reader["ClientEmail"],
                reader["ClientPhoneNumber"] == DBNull.Value
                    ? null
                    : (string)reader["ClientPhoneNumber"]);

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
                reader["SupplierAddress"] == DBNull.Value
                    ? string.Empty
                    : (string)reader["SupplierAddress"],
                reader["SupplierEmail"] == DBNull.Value
                    ? string.Empty
                    : (string)reader["SupplierEmail"],
                reader["SupplierPhoneNumber"] == DBNull.Value
                    ? string.Empty
                    : (string)reader["SupplierPhoneNumber"]);
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
        DateTime dueDate = (DateTime)reader["DueDate"];
        PaymentMethod method = (PaymentMethod)(byte)reader["PaymentMethod"];

        Payment payment = method switch
        {
            PaymentMethod.Cash =>
                new CashPayment(invoice, amount, dueDate),

            PaymentMethod.Card =>
                new CardPayment(
                    reader["Last4digits"] == DBNull.Value
                        ? throw new InvalidOperationException(
                            "A card payment has no last-four-digits value.")
                        : (string)reader["Last4digits"],
                    invoice,
                    amount,
                    dueDate),

            PaymentMethod.BankTransfer =>
                new BankTransferPayment(invoice, amount, dueDate),

            PaymentMethod.Crypto =>
                new CryptoPayment(invoice, amount, dueDate),

            _ => throw new InvalidOperationException(
                $"Unknown payment method: {method}")
        };

        if (reader["TransferReference"] != DBNull.Value &&
            payment is BankTransferPayment bankTransfer)
        {
            bankTransfer.SetTransferReference(
                (string)reader["TransferReference"]);
        }

        if (reader["TransactionReference"] != DBNull.Value &&
            payment is CryptoPayment crypto)
        {
            crypto.SetTransactionReference(
                (string)reader["TransactionReference"]);
        }

        if (reader["PaidDate"] != DBNull.Value)
        {
            payment.MarkAsPaid((DateTime)reader["PaidDate"]);
        }

        payment.SetId((int)reader["PaymentID"]);

        return payment;
    }
    public async Task<int> AddAsync(Payment item)
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

        string? last4Digits =
            item is CardPayment card ? card.Last4Digits : null;

        string? transferReference =
            item is BankTransferPayment bank ? bank.TransferReference : null;

        string? transactionReference =
            item is CryptoPayment crypto ? crypto.TransactionReference : null;

        const string query = @"
            INSERT INTO Payments
            (
                PurchaseInvoiceID,
                SalesInvoiceID,
                Amount,
                DueDate,
                PaidDate,
                PaymentMethod,
                Last4digits,
                TransferReference,
                TransactionReference
            )
            OUTPUT INSERTED.PaymentID
            VALUES
            (
                @PurchaseInvoiceID,
                @SalesInvoiceID,
                @Amount,
                @DueDate,
                @PaidDate,
                @PaymentMethod,
                @Last4digits,
                @TransferReference,
                @TransactionReference
            );";

        List<QueryParameter> parameters = new()
        {
            new("@PurchaseInvoiceID", SqlDbType.Int, purchaseInvoiceId),
            new("@SalesInvoiceID", SqlDbType.Int, salesInvoiceId),
            new("@Amount", SqlDbType.Decimal, item.Amount),
            new("@DueDate", SqlDbType.DateTime2, item.DueDate),
            new("@PaidDate", SqlDbType.DateTime2, item.PaidDate),
            new("@PaymentMethod", SqlDbType.TinyInt, (byte)item.Method),
            new("@Last4digits", SqlDbType.NVarChar, last4Digits),
            new("@TransferReference", SqlDbType.NVarChar, transferReference),
            new("@TransactionReference", SqlDbType.NVarChar, transactionReference)
        };

        int id = await ExecuteScalarAsync<int>(query, parameters);

        item.SetId(id);

        return id;
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

    public async Task<IReadOnlyList<Payment>> GetBySalesInvoiceIdAsync(
        int invoiceId)
    {
        return await QueryAsync(
            SelectQuery + " WHERE pay.SalesInvoiceID = @InvoiceID",
            new() { new("@InvoiceID", SqlDbType.Int, invoiceId) },
            BuildTable);
    }

    public async Task<IReadOnlyList<Payment>> GetByPurchaseInvoiceIdAsync(
        int invoiceId)
    {
        return await QueryAsync(
            SelectQuery + " WHERE pay.PurchaseInvoiceID = @InvoiceID",
            new() { new("@InvoiceID", SqlDbType.Int, invoiceId) },
            BuildTable);
    }

    public async Task<bool> MarkAsPaidAsync(int paymentId, DateTime paidDate)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(paymentId, 0);

        const string query = @"
        UPDATE Payments
        SET PaidDate = @PaidDate
        WHERE PaymentID = @PaymentID
          AND PaidDate IS NULL;";

        List<QueryParameter> parameters = new()
    {
        new("@PaidDate", SqlDbType.DateTime2, paidDate),
        new("@PaymentID", SqlDbType.Int, paymentId)
    };

        return await ExecuteAsync(query, parameters) > 0;
    }
}