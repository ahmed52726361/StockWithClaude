using Inventory.Domain.Contacts;
using Inventory.Domain.Orders.Invoices;
using Inventory.Domain.Orders.Lines.Abstract;
using Inventory.Domain.Products;

namespace Inventory.Domain.Orders.Lines
{
    public class SalesOrderLine:AbsOrderLine<Client>
    {
        public SalesOrderLine(SalesInvoice salesInvoice, Product product, int orderQty, decimal orderPrice) : base(salesInvoice, product, orderQty, orderPrice)
        {

        }
    }
}
