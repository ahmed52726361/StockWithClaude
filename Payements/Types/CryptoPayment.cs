using Inventory.Orders.Invoices.Interfaces;
using Inventory.Payements.Abstract;

namespace Inventory.Payements.Types
{
    internal sealed class CryptoPayment : Payment
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