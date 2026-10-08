using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Orders.Invoices
{
    internal interface IInvoice
    {
        int InvoiceId { get; }
        decimal InvoiceAmount { get; }
    }
}
