using Inventory.Contacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Orders.Invoices
{
    internal class SalesInvoice:AbsInvoice<Client>
    {
        public SalesInvoice(Client client,DateTime saleInvoiceDate) : base( client, saleInvoiceDate)
        {

        }
    }
}
