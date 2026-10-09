using Inventory.Contacts;
using Inventory.Orders.Invoices;
using Inventory.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Inventory.Orders.Lines
{

    internal class PurchaseOrderLine:AbsOrderLine<Supplier>
    {
        public PurchaseOrderLine( PurchaseInvoice purchaseInvoice,Product product,int ordreQty,decimal orderPrice) : base( purchaseInvoice, product, ordreQty, orderPrice)
        {

        }
    }
}
