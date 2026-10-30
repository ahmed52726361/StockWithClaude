using Inventory.General_Interfaces.Repository_Interfaces;
using Inventory.Orders.Lines;

namespace Inventory.General_Interfaces_and_abstract.Repository_Interfaces
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
