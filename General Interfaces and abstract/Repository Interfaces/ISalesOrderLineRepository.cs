using Inventory.General_Interfaces.Repository_Interfaces;
using Inventory.Orders.Lines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.General_Interfaces_and_abstract.Repository_Interfaces
{
    internal interface ISalesOrderLineRepository:IRepository<SalesOrderLine>
    {
        Task<IReadOnlyList<SalesOrderLine>> FindByInvoiceID(int invoiceID);
        Task<IReadOnlyList<SalesOrderLine>> FindByProductID(int productID);
        Task<IReadOnlyList<SalesOrderLine>> GetAllOrdersQtyLessThan(int qty);
        Task<IReadOnlyList<SalesOrderLine>> GetAllOrdersQtyGreaterThan(int qty);
        Task<IReadOnlyList<SalesOrderLine>> GetAllOrdersPriceLessThan(decimal price);
        Task<IReadOnlyList<SalesOrderLine>> GetAllOrdersPriceGreaterThan(decimal price);




    }
}
