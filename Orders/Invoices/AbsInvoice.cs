using Inventory.Contacts;
using Inventory.Orders.Lines;
using System;
using System.Collections.Generic;

namespace Inventory.Orders.Invoices
{
    internal abstract class AbsInvoice<TContact> : IInvoice where TContact : Contact
    {
        private readonly List<AbsOrderLine<TContact>> _lines = new List<AbsOrderLine<TContact>>();

        public int InvoiceId { get; }
        public TContact Contact { get; }
        public DateTime InvoiceDate { get; } // get-only — set once, in the constructor, never changes after

        public decimal InvoiceAmount
        {
            get
            {
                decimal amount = 0;
                foreach (var line in _lines)
                {
                    amount += line.OrderPrice * line.Qty;
                }
                return amount;
            }
        }

        public enum Status { Pending, Shipped, Completed, Cancelled }
        public Status CurrentStatus { get; private set; }

        public AbsInvoice(int invoiceId, TContact contact, DateTime invoiceDate)
        {
            InvoiceId = invoiceId;
            Contact = contact ?? throw new ArgumentNullException(nameof(contact));
            InvoiceDate = invoiceDate;
            CurrentStatus = Status.Pending;
        }

        // Controlled way to add a line — outside code can never replace the whole list
        public void AddLine(AbsOrderLine<TContact> line)
        {
            if (line == null)
            {
                throw new ArgumentNullException(nameof(line));
            }
            _lines.Add(line);
        }

        public IReadOnlyList<AbsOrderLine<TContact>> GetLines()
        {
            return _lines.AsReadOnly();
        }

        public void MarkAsShipped()
        {
            if (CurrentStatus != Status.Pending)
            {
                throw new InvalidOperationException($"Cannot ship an invoice with status {CurrentStatus}.");
            }
            CurrentStatus = Status.Shipped;
        }
        public void MarkCanceled()
        {
            if (CurrentStatus != Status.Completed)
            {
                throw new InvalidOperationException($"Cannot cancele an invoice with status {CurrentStatus}.");
            }
            CurrentStatus = Status.Cancelled;
        }
        public void MarkCompleted()
        {
            if (CurrentStatus != Status.Shipped)
            {
                throw new InvalidOperationException($"Cannot complete an invoice with status {CurrentStatus}.");
            }
            CurrentStatus = Status.Completed;
        }

    }
}