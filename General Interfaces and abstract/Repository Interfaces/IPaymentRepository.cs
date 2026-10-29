using Inventory.General_Interfaces.Repository_Interfaces;
using Inventory.General_Interfaces_and_abstract.General_abstract;
using Inventory.Payements.Abstract;
using System;
using System.Threading.Tasks;

namespace Inventory.General_Interfaces_and_abstract.Repository_Interfaces
{
    internal interface IPaymentRepository : IRepository<Payment>
    {
        Task<IReadOnlyList<Payment>> GetBySalesInvoiceIdAsync(int invoiceId);

        Task<IReadOnlyList<Payment>> GetByPurchaseInvoiceIdAsync(int invoiceId);
        Task<bool> MarkAsPaidAsync(int paymentId, DateTime paidDate);
    }
}