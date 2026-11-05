using Inventory.Domain.Contacts;
using Inventory.Domain.Orders.Invoices.Abstract;

namespace Inventory.Domain.Orders.Invoices
{
    public class PurchaseInvoice:AbsInvoice<Supplier>
    {
        public PurchaseInvoice(Supplier supplier,DateTime invoiceDate) : base(supplier,invoiceDate)
        {

        }
    }
}
