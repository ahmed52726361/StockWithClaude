using Inventory.Orders.Invoices.Interfaces;
using Inventory.Payements.Abstract;

namespace Inventory.Payements.Types
{
    internal sealed class CashPayment : Payment
    {
        public CashPayment(
            IInvoice invoice,
            decimal amount,
            DateTime dueDate)
            : base(invoice, amount, dueDate, PaymentMethod.Cash)
        {
        }
    }
}