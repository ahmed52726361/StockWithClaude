using Inventory.Domain.Orders.Invoices;

namespace Inventory.Domain.Repository_Interfaces
{
    public interface IPurchaseInvoiceRepository:IRepository<PurchaseInvoice>
    {
        Task<IReadOnlyList<PurchaseInvoice>> FindBySupplierName(string name);
        Task<IReadOnlyList<PurchaseInvoice>> FindBySupplierAddress(string address);
        Task<IReadOnlyList<PurchaseInvoice>> FindBySupplierEmail(string email);
        Task<IReadOnlyList<PurchaseInvoice>> FindBySupplierPhoneNumber(string phoneNumber);
        Task<IReadOnlyList<PurchaseInvoice>> GetAllInvoicesBySupplierID(int  supplierID);
        Task<IReadOnlyList<PurchaseInvoice>> GetAllInvoicesByDate(DateTime dateTime);

    }
}
