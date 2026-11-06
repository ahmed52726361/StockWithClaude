using Inventory.Domain.Orders.Invoices;

namespace Inventory.Domain.Repository_Interfaces
{
    public interface ISaleInvoiceRepository:IRepository<SalesInvoice>
    {
        Task<IReadOnlyList<SalesInvoice>> FindByClientNameAsync(string name);
        Task<IReadOnlyList<SalesInvoice>> FindByClientAddressAsync(string address);
        Task<IReadOnlyList<SalesInvoice>> FindByClientEmailAsync(string email);
        Task<IReadOnlyList<SalesInvoice>> FindByClientPhoneNumberAsync(string phoneNumber);
        Task<IReadOnlyList<SalesInvoice>> GetAllInvoicesByClientIDAsync(int clientId);
        Task<IReadOnlyList<SalesInvoice>> GetAllInvoicesByDateAsync(DateTime dateTime);

    }
}
