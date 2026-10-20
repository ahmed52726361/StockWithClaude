using Inventory.General_Interfaces.Repository_Interfaces;
using Inventory.Orders.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.General_Interfaces_and_abstract.Repository_Interfaces
{
    internal interface ISaleInvoiceRepository:IRepository<SalesInvoice>
    {
        Task<IReadOnlyList<SalesInvoice>> FindByClientName(string name);
        Task<IReadOnlyList<SalesInvoice>> FindByClientAddress(string address);
        Task<IReadOnlyList<SalesInvoice>> FindByClientEmail(string email);
        Task<IReadOnlyList<SalesInvoice>> FindByClientPhoneNumber(string phoneNumber);
        Task<IReadOnlyList<SalesInvoice>> GetAllInvoicesByClientID(int clientId);
        Task<IReadOnlyList<SalesInvoice>> GetAllInvoicesByDate(DateTime dateTime);

    }
}
