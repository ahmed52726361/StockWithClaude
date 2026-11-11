using Inventory.Application.Exceptions;
using Inventory.Domain.Contacts;
using Inventory.Domain.Orders.Invoices;
using Inventory.Domain.Repository_Interfaces;

namespace Inventory.Application
{
    internal class SaleService
    {
        private readonly ISaleInvoiceRepository _invoiceRepository;
        private readonly ClientService _clientService;
        public SaleService(ISaleInvoiceRepository invoiceRepository,ClientService clientRepository)
        {
            _invoiceRepository = invoiceRepository;
            _clientService = clientRepository;
        }


        ///<summary>Adds a new SaleInvoice</summary>
        ///<exception cref="ClientNotFoundException">throw when clientName not found</exception>
        ///
        public async Task<int> AddSaleInvoice(int  clientID,DateTime date)
        {
           Client client=await _clientService.FindByIDAsync(clientID);
            if (client==null)
            {
                throw new ClientNotFoundException(clientID);
            }
            SalesInvoice salesInvoice=new(client,date);
            return await _invoiceRepository.AddAsync(salesInvoice);

        }
        public async Task<SalesInvoice>FindSaleInvoice(int saleInvoiceId)
        {
            SalesInvoice? salesInvoice =await _invoiceRepository.GetByIdAsync(saleInvoiceId);
            if (salesInvoice==null)
            {
                throw new InvoiceNotFoundException(saleInvoiceId);
            }
            return salesInvoice;
        }

    
    }



}
