using Inventory.Contacts;
using Inventory.Orders.Invoices.Abstract;

namespace Inventory.Orders.Invoices
{
    public class PurchaseInvoice:AbsInvoice<Supplier>
    {
        public PurchaseInvoice(Supplier supplier,DateTime invoiceDate) : base(supplier,invoiceDate)
        {

        }
    }
}
