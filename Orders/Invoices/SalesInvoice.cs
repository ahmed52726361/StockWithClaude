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
        public SalesInvoice(int saleInvoiceID,Client client,DateTime saleInvoiceDate) : base(saleInvoiceID, client, saleInvoiceDate)
        {

        }
    }
}
