using Inventory.Orders.Invoices.Interfaces;
using Inventory.Payements.Abstract;

namespace Inventory.Payements.Types
{
    internal sealed class BankTransferPayment : Payment
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