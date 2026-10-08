using Inventory.Contacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Orders.Invoices
{
    internal class PurchaseInvoice:AbsInvoice<Supplier>
    {
        public PurchaseInvoice(int invoiceId,Supplier supplier,DateTime invoiceDate) : base(invoiceId,supplier,invoiceDate)
        {

        }
    }
}
