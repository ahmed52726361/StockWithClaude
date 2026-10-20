using Inventory.Contacts.Abstract;
using Inventory.General_Interfaces;
using Inventory.Orders.Invoices.Interfaces;
using Inventory.Orders.Lines.Abstract;
using Inventory.Services;
using System;
using System.Collections.Generic;

namespace Inventory.Orders.Invoices.Abstract
{
    internal abstract class AbsInvoice<TContact>
        : IInvoice, IReceivesDatabaseId
        where TContact : AbsContact
    {
        private readonly List<AbsOrderLine<TContact>> _lines = new List<AbsOrderLine<TContact>>();

        public int Id {  get; private set; }
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

        public int InvoiceId => Id;

        public AbsInvoice( TContact contact, DateTime invoiceDate)
        {
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
            else if (line.Invoice!=this)
            {
                throw new ArgumentException("You can't add a record line that belongs to another invoice.");
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
            if (CurrentStatus != Status.Shipped && CurrentStatus != Status.Pending)
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

        public void SetId(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Id should be positive");
            }
            else if (Id > 0)
            {
                throw new InvalidOperationException(
                    "The record already has an ID.");
            }
            Id = id;
        }
        public void SetStatus(Status status)
        {
            CurrentStatus = status;
        }
    }
}