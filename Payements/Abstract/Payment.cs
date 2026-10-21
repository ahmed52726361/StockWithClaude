using Inventory.General_Interfaces;
using Inventory.Orders.Invoices.Interfaces;
using Inventory.Services;
using System;

namespace Inventory.Payements.Abstract
{
    internal abstract class Payment : IReceivesDatabaseId
    {
        public int Id {get; private set;}
        private decimal _amount;
        public enum PaymentStatus
        {
            Unpaid,
            Pending,
            Paid,
            Refunded
        }
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


        public Payment(IInvoice invoice, decimal amount, DateTime date)
        {
            Invoice = invoice ?? throw new ArgumentNullException(nameof(invoice));
            Amount = amount;
            Date = date;
        }

        public virtual string ReceiptText()
        {
            return $"Payment of {Amount} on {Date} for invoice #{Invoice.InvoiceId}";
        }

        public abstract string Pay();


        protected string ProcessPayment(string successMessage)
        {
            if (Status != PaymentStatus.Unpaid)
            {
                return $"Cannot pay a payment with status {Status}.";
            }

            Status = PaymentStatus.Paid;
            return successMessage;
        }

        protected string MarkPending(string message)
        {
            if (Status != PaymentStatus.Unpaid)
            {
                return $"Cannot initiate a transfer with status {Status}.";
            }

            Status = PaymentStatus.Pending;
            return message;
        }

        protected string ConfirmPendingPayment(string message)
        {
            if (Status != PaymentStatus.Pending)
            {
                return $"Cannot confirm a payment with status {Status}.";
            }

            Status = PaymentStatus.Paid;
            return message;
        }

        protected string ProcessRefund(string successMessage)
        {
            if (Status != PaymentStatus.Paid)
            {
                return $"Cannot refund a payment with status {Status}.";
            }

            Status = PaymentStatus.Refunded;
            return successMessage;
        }

        public void SetId(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Id should be positive");
            }
            else if (Id > 0)
            {
                throw new InvalidOperationException(
                    "The payment already has an ID.");
            }
            Id = id;
        }
        public void SetStatus(PaymentStatus status)
        {
            if (!Enum.IsDefined(status))
                throw new ArgumentOutOfRangeException(nameof(status));

            Status = status;
        }
    }
}