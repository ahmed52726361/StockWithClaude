using Inventory.Contacts.Abstract;
using Inventory.General_Interfaces;
using Inventory.Orders.Invoices.Abstract;
using Inventory.Products;

namespace Inventory.Orders.Lines.Abstract
{
    public abstract class AbsOrderLine<TContact>
        : IReceivesDatabaseId
        where TContact : AbsContact
    {
        public int Id {  get; private set; }
        public AbsInvoice<TContact> Invoice { get; }
        public Product Product { get; }
        public int Qty { get; }
        public decimal OrderPrice { get; }


        public AbsOrderLine(
            AbsInvoice<TContact> invoice,
            Product product,
            int qty,
            decimal price)
        {
            Invoice = invoice ?? throw new ArgumentNullException(nameof(invoice));
            Product = product ?? throw new ArgumentNullException(nameof(product));
            Qty = qty <= 0 ? throw new ArgumentOutOfRangeException(nameof(qty), "Quantity should be positive") : qty;
            OrderPrice = price <= 0 ? throw new ArgumentOutOfRangeException(nameof(price), "Price should be positive") : price;
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
    }
}