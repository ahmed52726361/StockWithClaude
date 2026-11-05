using Inventory.Domain.Contacts;
using Inventory.Domain.Orders.Invoices;
using Inventory.Domain.Orders.Lines.Abstract;
using Inventory.Domain.Products;

namespace Inventory.Domain.Orders.Lines
{

    public class PurchaseOrderLine:AbsOrderLine<Supplier>
    {
        public PurchaseOrderLine( PurchaseInvoice purchaseInvoice,Product product,int ordreQty,decimal orderPrice) : base( purchaseInvoice, product, ordreQty, orderPrice)
        {

        }
    }
}
