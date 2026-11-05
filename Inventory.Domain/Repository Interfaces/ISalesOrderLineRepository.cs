using Inventory.Domain.Orders.Lines;

namespace Inventory.Domain.Repository_Interfaces
{
    public interface ISalesOrderLineRepository:IRepository<SalesOrderLine>
    {
        Task<IReadOnlyList<SalesOrderLine>> FindByInvoiceID(int invoiceID);
        Task<IReadOnlyList<SalesOrderLine>> FindByProductID(int productID);
        Task<IReadOnlyList<SalesOrderLine>> GetAllOrdersQtyLessThan(int qty);
        Task<IReadOnlyList<SalesOrderLine>> GetAllOrdersQtyGreaterThan(int qty);
        Task<IReadOnlyList<SalesOrderLine>> GetAllOrdersPriceLessThan(decimal price);
        Task<IReadOnlyList<SalesOrderLine>> GetAllOrdersPriceGreaterThan(decimal price);




    }
}
