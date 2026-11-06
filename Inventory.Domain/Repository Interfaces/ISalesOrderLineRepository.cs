using Inventory.Domain.Orders.Lines;

namespace Inventory.Domain.Repository_Interfaces
{
    public interface ISalesOrderLineRepository:IRepository<SalesOrderLine>
    {
        Task<IReadOnlyList<SalesOrderLine>> FindByInvoiceIDAsync(int invoiceID);
        Task<IReadOnlyList<SalesOrderLine>> FindByProductIDAsync(int productID);
        Task<IReadOnlyList<SalesOrderLine>> GetAllOrdersQtyLessThanAsync(int qty);
        Task<IReadOnlyList<SalesOrderLine>> GetAllOrdersQtyGreaterThanAsync(int qty);
        Task<IReadOnlyList<SalesOrderLine>> GetAllOrdersPriceLessThanAsync(decimal price);
        Task<IReadOnlyList<SalesOrderLine>> GetAllOrdersPriceGreaterThanAsync(decimal price);




    }
}
