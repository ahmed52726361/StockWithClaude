using Inventory.Domain.Orders.Invoices.Interfaces;
using Inventory.Domain.Payments.Abstract;

namespace Inventory.Domain.Payments.Types
{
    public sealed class BankTransferPayment : Payment
    {
        public string? TransferReference { get; private set; }

        public BankTransferPayment(
            IInvoice invoice,
            decimal amount,
            DateTime dueDate)
            : base(invoice, amount, dueDate, PaymentMethod.BankTransfer)
        {
        }

        public void SetTransferReference(string reference)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(reference);

            TransferReference = reference;
        }
    }
}