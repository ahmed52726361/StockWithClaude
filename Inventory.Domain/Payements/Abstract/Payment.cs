using Inventory.General_Interfaces;
using Inventory.Orders.Invoices.Interfaces;

namespace Inventory.Payements.Abstract
{
    public abstract class Payment : IReceivesDatabaseId
    {
        public int Id { get; private set; }

        public IInvoice Invoice { get; }

        public decimal Amount { get; }

        public DateTime DueDate { get; }

        public DateTime? PaidDate { get; private set; }

        public PaymentMethod Method { get; }

        public PaymentStatus Status
        {
            get
            {
                if (PaidDate.HasValue)
                    return PaymentStatus.Paid;

                DateTime today = DateTime.Today;

                if (today > DueDate.Date)
                    return PaymentStatus.Overdue;

                if (today >= DueDate.Date.AddDays(-10))
                    return PaymentStatus.Pending;

                return PaymentStatus.Unpaid;
            }
        }

        protected Payment(
            IInvoice invoice,
            decimal amount,
            DateTime dueDate,
            PaymentMethod method)
        {
            Invoice = invoice
                ?? throw new ArgumentNullException(nameof(invoice));

            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            if (!Enum.IsDefined(method))
                throw new ArgumentOutOfRangeException(nameof(method));

            Amount = amount;
            DueDate = dueDate;
            Method = method;
        }

        public void MarkAsPaid(DateTime paidDate)
        {
            if (PaidDate.HasValue)
                throw new InvalidOperationException(
                    "This payment has already been paid.");

            if (paidDate.Date > DateTime.Today)
                throw new ArgumentOutOfRangeException(nameof(paidDate));

            PaidDate = paidDate;
        }

        public virtual string ReceiptText()
        {
            return $"Payment #{Id}, Amount: {Amount}, " +
                   $"Due: {DueDate:d}, Method: {Method}";
        }

        public void SetId(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            if (Id > 0)
                throw new InvalidOperationException(
                    "This payment already has an ID.");

            Id = id;
        }
    }
}