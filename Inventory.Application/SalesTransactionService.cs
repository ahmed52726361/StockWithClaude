
using Inventory.Application.Exceptions;
using Inventory.Domain.Contacts;
using Inventory.Domain.Orders.Invoices;
using Inventory.Domain.Orders.Lines;
using Inventory.Domain.Products;
using Inventory.Domain.Repository_Interfaces;

namespace Inventory.Application
{
    public class SalesTransactionService
    {
        private readonly ProductService _productService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ClientService _clientService;
        private readonly ISaleInvoiceRepository _saleInvoiceRepository;
        private readonly ISalesOrderLineRepository _salesOrderLineRepository;

        public SalesTransactionService(
            IProductRepository productRepository,
            IUnitOfWork unitOfWork,
            ClientService clientService,
            ISaleInvoiceRepository saleInvoiceRepository,
            ISalesOrderLineRepository salesOrderLineRepository)
        {
            _productService = new ProductService(productRepository);
            _unitOfWork = unitOfWork;
            _clientService = clientService;
            _saleInvoiceRepository = saleInvoiceRepository;
            _salesOrderLineRepository = salesOrderLineRepository;
        }
        public async Task<int> SellProductsAsync(int clientId, List<SaleItem> items)
        {
            if (items == null || items.Count == 0)
                throw new ArgumentException(
                    "A sale needs at least one item.",
                    nameof(items));

            // PHASE 1: Read and validate in memory. No writes.

            Client client = await _clientService.FindByIDAsync(clientId);

            SalesInvoice invoice = new(client, DateTime.Now);

            List<SalesOrderLine> lines = new();

            // Each product is loaded only once.
            Dictionary<int, Product> products = new();
                
            foreach (SaleItem item in items)
            {
                if (!products.TryGetValue(item.ProductID, out Product? product))
                {
                    product = await _productService.FindProductAsync(item.ProductID);
                    products[item.ProductID] = product;
                }

                // Checks stock and reduces it in memory.
                product.Sale(item.OrderQuantity);

                SalesOrderLine line = new(
                    invoice,
                    product,
                    item.OrderQuantity,
                    item.ProductPrice);

                invoice.AddLine(line);
                lines.Add(line);
            }

            // PHASE 2: Perform all database writes in one transaction.

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                int invoiceId = await _saleInvoiceRepository.AddAsync(invoice);

                foreach (SalesOrderLine line in lines)
                    await _salesOrderLineRepository.AddAsync(line);

                foreach (Product product in products.Values)
                    await _productService.UpdateProductAsync(product);

                await _unitOfWork.CommitAsync();

                return invoiceId;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
            finally
            {
                await _unitOfWork.DisposeAsync();
            }
        }
    }
}
