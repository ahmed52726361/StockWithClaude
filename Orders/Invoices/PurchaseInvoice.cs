using Inventory.Contacts;
using Inventory.Orders.Invoices.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Orders.Invoices
{
    internal class PurchaseInvoice:AbsInvoice<Supplier>
    {
        public PurchaseInvoice(Supplier supplier,DateTime invoiceDate) : base(supplier,invoiceDate)
        {

        }
    }
}
