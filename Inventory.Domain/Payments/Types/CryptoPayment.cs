using Inventory.Domain.Orders.Invoices.Interfaces;
using Inventory.Domain.Payments.Abstract;

namespace Inventory.Domain.Payments.Types
{
    public sealed class CryptoPayment : Payment
    {
        public string? TransactionReference { get; private set; }

        public CryptoPayment(
            IInvoice invoice,
            decimal amount,
            DateTime dueDate)
            : base(invoice, amount, dueDate, PaymentMethod.Crypto)
        {
        }

        public void SetTransactionReference(string reference)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(reference);

            TransactionReference = reference;
        }
    }
}