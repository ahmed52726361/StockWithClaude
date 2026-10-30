using Inventory.General_Interfaces.Repository_Interfaces;
using Inventory.Orders.Invoices;

namespace Inventory.General_Interfaces_and_abstract.Repository_Interfaces
{
    public interface ISaleInvoiceRepository:IRepository<SalesInvoice>
    {
        Task<IReadOnlyList<SalesInvoice>> FindByClientName(string name);
        Task<IReadOnlyList<SalesInvoice>> FindByClientAddress(string address);
        Task<IReadOnlyList<SalesInvoice>> FindByClientEmail(string email);
        Task<IReadOnlyList<SalesInvoice>> FindByClientPhoneNumber(string phoneNumber);
        Task<IReadOnlyList<SalesInvoice>> GetAllInvoicesByClientID(int clientId);
        Task<IReadOnlyList<SalesInvoice>> GetAllInvoicesByDate(DateTime dateTime);

    }
}
