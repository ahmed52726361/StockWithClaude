using Inventory.Contacts;
using Inventory.Orders.Invoices.Abstract;

namespace Inventory.Orders.Invoices
{
    public class SalesInvoice:AbsInvoice<Client>
    {
        public SalesInvoice(Client client,DateTime saleInvoiceDate) : base( client, saleInvoiceDate)
        {

        }
    }
}
