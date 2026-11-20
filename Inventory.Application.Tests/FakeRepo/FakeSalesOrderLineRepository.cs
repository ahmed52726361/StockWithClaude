using Inventory.Domain.Orders.Invoices;
using Inventory.Domain.Orders.Lines;
using Inventory.Domain.Products;
using Inventory.Domain.Repository_Interfaces;

namespace Inventory.Application.Tests.FakeRepo
{
    internal class FakeSalesOrderLineRepository : FakeRepositoryBase<SalesOrderLine>, ISalesOrderLineRepository
    {
        protected override SalesOrderLine Copy(SalesOrderLine source)
        {
            SalesOrderLine clone = new(
                (SalesInvoice)source.Invoice,
                source.Product,
                source.Qty,
                source.OrderPrice

                );
                clone.SetId( source.Id );
            return clone;
        }

        public Task<IReadOnlyList<SalesOrderLine>> FindByInvoiceIDAsync(int invoiceID)
        {
            IReadOnlyList<SalesOrderLine> result = Items.Where(o=>o.Invoice.Id == invoiceID).Select(o=>Copy(o)).ToList();
            return Task.FromResult(result);
        }

        public Task<IReadOnlyList<SalesOrderLine>> FindByProductIDAsync(int productID)
        {
            IReadOnlyList<SalesOrderLine> result = Items.Where(o => o.Product.Id == productID).Select(o => Copy(o)).ToList();
            return Task.FromResult(result);
        }

        public Task<IReadOnlyList<SalesOrderLine>> GetAllOrdersPriceGreaterThanAsync(decimal price)
        {
            IReadOnlyList<SalesOrderLine> result = Items.Where(o => o.OrderPrice>price).Select(o => Copy(o)).ToList();
            return Task.FromResult(result);
        }

        public Task<IReadOnlyList<SalesOrderLine>> GetAllOrdersPriceLessThanAsync(decimal price)
        {
            IReadOnlyList<SalesOrderLine> result = Items.Where(o => o.OrderPrice<price).Select(o => Copy(o)).ToList();
            return Task.FromResult(result);
        }

        public Task<IReadOnlyList<SalesOrderLine>> GetAllOrdersQtyGreaterThanAsync(int qty)
        {
            IReadOnlyList<SalesOrderLine> result = Items.Where(o => o.Qty>qty).Select(o => Copy(o)).ToList();
            return Task.FromResult(result); 
        }

        public Task<IReadOnlyList<SalesOrderLine>> GetAllOrdersQtyLessThanAsync(int qty)
        {
            IReadOnlyList<SalesOrderLine> result = Items.Where(o => o.Qty<qty).Select(o => Copy(o)).ToList();
            return Task.FromResult(result);
        }

    }
}
