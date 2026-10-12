using Inventory.Contacts;
using Inventory.Orders.Invoices;
using Inventory.Orders.Lines.Abstract;
using Inventory.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Orders.Lines
{
    internal class SalesOrderLine:AbsOrderLine<Client>
    {
        public SalesOrderLine(SalesInvoice salesInvoice, Product product, int orderQty, decimal orderPrice) : base(salesInvoice, product, orderQty, orderPrice)
        {

        }
    }
}
