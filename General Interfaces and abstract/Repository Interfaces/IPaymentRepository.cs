using Inventory.General_Interfaces.Repository_Interfaces;
using Inventory.Payements.Abstract;

namespace Inventory.General_Interfaces_and_abstract.Repository_Interfaces;

internal interface IPaymentRepository : IRepository<Payment>
{
    Task<IReadOnlyList<Payment>> GetByInvoiceIdAsync(int invoiceId);
}