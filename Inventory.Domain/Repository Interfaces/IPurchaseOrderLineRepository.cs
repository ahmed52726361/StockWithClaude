using Inventory.Domain.Orders.Lines;

namespace Inventory.Domain.Repository_Interfaces
{
    public interface IPurchaseOrderLineRepository:IRepository<PurchaseOrderLine>
    {
        Task<IReadOnlyList<PurchaseOrderLine>> FindByInvoiceID(int invoiceID);
        Task<IReadOnlyList<PurchaseOrderLine>> FindByProductID(int productID);
        Task<IReadOnlyList<PurchaseOrderLine>> GetAllOrdersQtyLessThan(int qty);
        Task<IReadOnlyList<PurchaseOrderLine>> GetAllOrdersQtyGreaterThan(int qty);
        Task<IReadOnlyList<PurchaseOrderLine>> GetAllOrdersPriceLessThan(decimal price);
        Task<IReadOnlyList<PurchaseOrderLine>> GetAllOrdersPriceGreaterThan(decimal price);

    }
}
