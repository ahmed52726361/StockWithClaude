using Inventory.General_Interfaces.Repository_Interfaces;
using Inventory.Payements.Abstract;

namespace Inventory.General_Interfaces_and_abstract.Repository_Interfaces
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<IReadOnlyList<Payment>> GetBySalesInvoiceIdAsync(int invoiceId);

        Task<IReadOnlyList<Payment>> GetByPurchaseInvoiceIdAsync(int invoiceId);
        Task<bool> MarkAsPaidAsync(int paymentId, DateTime paidDate);
    }
}