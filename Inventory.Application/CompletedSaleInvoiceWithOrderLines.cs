using Inventory.Domain.Orders.Lines;
using Inventory.Domain.Repository_Interfaces;

namespace Inventory.Application
{
    public class CompletedSaleInvoiceWithOrderLines
    {
        List<SalesOrderLine> _salesOrderLines;
        private SaleService _SaleService;
        private OrderService _OrderService;
        private ProductService _ProductService;
        private IUnitOfWork _unitofWork;

        public CompletedSaleInvoiceWithOrderLines(IUnitOfWork unitofWork,ISaleInvoiceRepository saleInvoiceRepository
            ,IProductRepository productRepository,IClientRepository clientRepository,ISalesOrderLineRepository salesOrderLineRepository)
        {
            ClientService clientService = new ClientService(clientRepository);  
            _unitofWork = unitofWork;
            _SaleService=new(saleInvoiceRepository,clientService);
            _ProductService = new(productRepository);
            _OrderService = new(salesOrderLineRepository,_SaleService, _ProductService);
        }
        public async Task<int> CreateSaleInvoiceWithOrderLines(int ClientID, List<ListProductsToSale> listProductsToSales)
        {
            int SaleID;
            await _unitofWork.BeginTransactionAsync();
            try
            {
                SaleID= await _SaleService.AddSaleInvoice(ClientID, DateTime.Now);
                foreach(var product in listProductsToSales)
                {
                   await _OrderService.AddSaleOrder(new(await _SaleService.FindSaleInvoice(SaleID),await _ProductService.FindProductAsync(product.ProductID), product.OrderQuantity, product.ProductPrice));
                }
                await _unitofWork.CommitAsync();
            }
            catch
            {
                await _unitofWork.RollbackAsync();
                throw;
            }finally
            {
                 
                await _unitofWork.DisposeAsync();
            }
            return SaleID;
        }
    }
}
