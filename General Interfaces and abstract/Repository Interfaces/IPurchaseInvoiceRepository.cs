using Inventory.Contacts;
using Inventory.General_Interfaces.Repository_Interfaces;
using Inventory.Orders.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.General_Interfaces_and_abstract.Repository_Interfaces
{
    internal interface IPurchaseInvoiceRepository:IRepository<PurchaseInvoice>
    {
        Task<IReadOnlyList<PurchaseInvoice>> FindBySupplierName(string name);
        Task<IReadOnlyList<PurchaseInvoice>> FindBySupplierAddress(string address);
        Task<IReadOnlyList<PurchaseInvoice>> FindBySupplierEmail(string email);
        Task<IReadOnlyList<PurchaseInvoice>> FindBySupplierPhoneNumber(string phoneNumber);
        Task<IReadOnlyList<PurchaseInvoice>> GetAllInvoicesBySupplierID(int  supplierID);
        Task<IReadOnlyList<PurchaseInvoice>> GetAllInvoicesByDate(DateTime dateTime);

    }
}
