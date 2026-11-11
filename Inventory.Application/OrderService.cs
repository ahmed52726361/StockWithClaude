using Inventory.Application.Exceptions;
using Inventory.Domain.Orders.Lines;
using Inventory.Domain.Products;
using Inventory.Domain.Repository_Interfaces;

namespace Inventory.Application
{
    internal class OrderService
    {
        private readonly ISalesOrderLineRepository _salesOrderLineRepository;
        private readonly SaleService _saleService;
        private readonly ProductService _productService;
        public OrderService(ISalesOrderLineRepository salesOrderLineRepository, SaleService saleService, ProductService productService)
        {
            _salesOrderLineRepository = salesOrderLineRepository;
            _saleService = saleService;
            _productService = productService;
        }

        public async Task<int>AddSaleOrder(SalesOrderLine orderLine)
        {
            if (await _productService.FindProductAsync(orderLine.Product.Id)==null||await _saleService.FindSaleInvoice(orderLine.Invoice.InvoiceId)==null)
            {
                throw new AddOrderException(orderLine.Product.Name, orderLine.Invoice.InvoiceId);
            }
            return await _salesOrderLineRepository.AddAsync(orderLine);
        }
    }
}
