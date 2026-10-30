using Inventory.Contacts;
using Inventory.Orders.Invoices;
using Inventory.Orders.Lines.Abstract;
using Inventory.Products;

namespace Inventory.Orders.Lines
{
    public class SalesOrderLine:AbsOrderLine<Client>
    {
        public SalesOrderLine(SalesInvoice salesInvoice, Product product, int orderQty, decimal orderPrice) : base(salesInvoice, product, orderQty, orderPrice)
        {

        }
    }
}
