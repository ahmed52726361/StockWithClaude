using Inventory.Orders.Invoices;
using Inventory.Services;
using System;

namespace Inventory.Payements
{
    internal abstract class Payment : IHasId
    {
        public int PaymentId { get; }
        private decimal _amount;
        public enum PaymentStatus { Unpaid, Paid, Refunded }
        public PaymentStatus Status { get; protected set; } = PaymentStatus.Unpaid;
        public decimal Amount
        {
            get { return _amount; }
            private set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(Amount), "Amount should be positive");
                }
                else
                {
                    _amount = value;
                }
            }
        }
        public DateTime Date { get; private set; }
        public IInvoice Invoice { get; }

        public int Id => PaymentId;

        public Payment(int paymentId, IInvoice invoice, decimal amount, DateTime date)
        {
            PaymentId = paymentId;
            Invoice = invoice ?? throw new ArgumentNullException(nameof(invoice));
            Amount = amount;
            Date = date;
        }

        public virtual string ReceiptText()
        {
            return $"Payment of {Amount} on {Date} for invoice #{Invoice.InvoiceId}";
        }

        public abstract string Pay();

        protected string ProcessPayment(string msg)
        {
            if (Status == PaymentStatus.Unpaid)
            {
                Status = PaymentStatus.Paid;
                return msg;
            }
            else
            {
                return "Payments already paid";
            }
        }

        protected string ProcessRefund(string successMessage)
        {
            if (Status == PaymentStatus.Paid)
            {
                Status = PaymentStatus.Refunded;
                return successMessage;
            }
            else if (Status == PaymentStatus.Unpaid)
            {
                return "was never paid.";
            }
            else
            {
                return "already refunded.";
            }
        }
    }
}