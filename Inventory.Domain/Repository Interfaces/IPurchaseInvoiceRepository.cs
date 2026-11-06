using Inventory.Domain.Orders.Invoices;

namespace Inventory.Domain.Repository_Interfaces
{
    public interface IPurchaseInvoiceRepository:IRepository<PurchaseInvoice>
    {
        Task<IReadOnlyList<PurchaseInvoice>> FindBySupplierNameAsync(string name);
        Task<IReadOnlyList<PurchaseInvoice>> FindBySupplierAddressAsync(string address);
        Task<IReadOnlyList<PurchaseInvoice>> FindBySupplierEmailAsync(string email);
        Task<IReadOnlyList<PurchaseInvoice>> FindBySupplierPhoneNumberAsync(string phoneNumber);
        Task<IReadOnlyList<PurchaseInvoice>> GetAllInvoicesBySupplierIDAsync(int  supplierID);
        Task<IReadOnlyList<PurchaseInvoice>> GetAllInvoicesByDateAsync(DateTime dateTime);

    }
}
