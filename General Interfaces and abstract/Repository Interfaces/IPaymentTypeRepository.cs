using Inventory.General_Interfaces.Repository_Interfaces;
using Inventory.Payements;

namespace Inventory.General_Interfaces_and_abstract.Repository_Interfaces;

internal interface IPaymentTypeRepository : IRepository<PaymentType>
{
    Task<PaymentType?> FindByNameAsync(string name);
}