using Inventory.General_Interfaces.Repository_Interfaces;
using Inventory.Orders.Lines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.General_Interfaces_and_abstract.Repository_Interfaces
{
    internal interface IPurchaseOrderLineRepository:IRepository<PurchaseOrderLine>
    {
        Task<IReadOnlyList<PurchaseOrderLine>> FindByInvoiceID(int invoiceID);
        Task<IReadOnlyList<PurchaseOrderLine>> FindByProductID(int productID);
        Task<IReadOnlyList<PurchaseOrderLine>> GetAllOrdersQtyLessThan(int qty);
        Task<IReadOnlyList<PurchaseOrderLine>> GetAllOrdersQtyGreaterThan(int qty);
        Task<IReadOnlyList<PurchaseOrderLine>> GetAllOrdersPriceLessThan(decimal price);
        Task<IReadOnlyList<PurchaseOrderLine>> GetAllOrdersPriceGreaterThan(decimal price);

    }
}
