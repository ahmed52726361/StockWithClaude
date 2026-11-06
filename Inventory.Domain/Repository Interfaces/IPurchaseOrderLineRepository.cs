using Inventory.Domain.Orders.Lines;

namespace Inventory.Domain.Repository_Interfaces
{
    public interface IPurchaseOrderLineRepository:IRepository<PurchaseOrderLine>
    {
        Task<IReadOnlyList<PurchaseOrderLine>> FindByInvoiceIDAsync(int invoiceID);
        Task<IReadOnlyList<PurchaseOrderLine>> FindByProductIDAsync(int productID);
        Task<IReadOnlyList<PurchaseOrderLine>> GetAllOrdersQtyLessThanAsync(int qty);
        Task<IReadOnlyList<PurchaseOrderLine>> GetAllOrdersQtyGreaterThanAsync(int qty);
        Task<IReadOnlyList<PurchaseOrderLine>> GetAllOrdersPriceLessThanAsync(decimal price);
        Task<IReadOnlyList<PurchaseOrderLine>> GetAllOrdersPriceGreaterThanAsync(decimal price);

    }
}
