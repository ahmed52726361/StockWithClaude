using Inventory.Domain.Contacts;
using Inventory.Domain.Orders.Invoices.Abstract;

namespace Inventory.Domain.Orders.Invoices
{
    public class SalesInvoice:AbsInvoice<Client>
    {
        public SalesInvoice(Client client,DateTime saleInvoiceDate) : base( client, saleInvoiceDate)
        {

        }
    }
}
