using Inventory.Contacts;
using Inventory.Orders.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Orders.Lines
{
    internal class SalesOrdersLine:AbsOrderLine<Client>
    {
        public SalesOrdersLine(int SalesOrderLineID,SalesInvoice salesInvoice,Product product,int orderQty,decimal orderPrice) : base(SalesOrderLineID, salesInvoice, product, orderQty, orderPrice)
        {

        }
    }
}
