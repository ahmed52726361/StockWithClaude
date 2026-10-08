using Inventory.Contacts;
using System;

namespace Inventory.Orders.Lines
{
    internal abstract class AbsOrderLine<TContact> where TContact : Contact
    {
        public int OrderID { get; }
        public Invoices.AbsInvoice<TContact> Invoice { get; }
        public Product Product { get; }
        public int Qty { get; }
        public decimal OrderPrice { get; }

        public AbsOrderLine(
            int id,
            Invoices.AbsInvoice<TContact> invoice,
            Product product,
            int qty,
            decimal price)
        {
            OrderID = id;
            Invoice = invoice ?? throw new ArgumentNullException(nameof(invoice));
            Product = product ?? throw new ArgumentNullException(nameof(product));
            Qty = qty;
            OrderPrice = price;
        }
    }
}