using Inventory.Domain.Orders.Invoices.Interfaces;
using Inventory.Domain.Payments.Abstract;

namespace Inventory.Domain.Payments.Types
{
    public sealed class CashPayment : Payment
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