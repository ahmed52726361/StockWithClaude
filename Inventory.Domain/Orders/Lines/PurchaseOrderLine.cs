using Inventory.Contacts;
using Inventory.Orders.Invoices;
using Inventory.Orders.Lines.Abstract;
using Inventory.Products;

namespace Inventory.Orders.Lines
{

    public class PurchaseOrderLine:AbsOrderLine<Supplier>
    {
        public PurchaseOrderLine( PurchaseInvoice purchaseInvoice,Product product,int ordreQty,decimal orderPrice) : base( purchaseInvoice, product, ordreQty, orderPrice)
        {

        }
    }
}
