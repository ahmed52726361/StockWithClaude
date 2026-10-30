using Inventory.Orders.Invoices.Interfaces;
using Inventory.Payements.Abstract;

namespace Inventory.Payements.Types
{
    public sealed class CardPayment : Payment
    {
        public string Last4Digits { get; }

        public CardPayment(
            string last4Digits,
            IInvoice invoice,
            decimal amount,
            DateTime dueDate)
            : base(invoice, amount, dueDate, PaymentMethod.Card)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(last4Digits);

            if (last4Digits.Length != 4 ||
                !last4Digits.All(char.IsDigit))
            {
                throw new ArgumentException(
                    "Enter exactly four digits.",
                    nameof(last4Digits));
            }

            Last4Digits = last4Digits;
        }

        public override string ReceiptText()
        {
            return base.ReceiptText() +
                   $" Card ending in {Last4Digits}";
        }
    }
}