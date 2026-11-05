using Inventory.Domain.Repository_Interfaces;
using Inventory.Domain.Payments.Abstract;

namespace Inventory.Domain.Repository_Interfaces
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<IReadOnlyList<Payment>> GetBySalesInvoiceIdAsync(int invoiceId);

        Task<IReadOnlyList<Payment>> GetByPurchaseInvoiceIdAsync(int invoiceId);
        Task<bool> MarkAsPaidAsync(int paymentId, DateTime paidDate);
    }
}