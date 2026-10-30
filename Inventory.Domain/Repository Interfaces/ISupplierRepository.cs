using Inventory.Contacts;
using Inventory.General_Interfaces.Repository_Interfaces;

namespace Inventory.General_Interfaces_and_abstract.Repository_Interfaces
{
    public interface ISupplierRepository : IRepository<Supplier>
    {
        Task<Supplier?> FindByName(string name);
        Task<Supplier?> FindByAddress(string address);
        Task<Supplier?> FindByEmail(string email);
        Task<Supplier?> FindByPhoneNumber(string phoneNumber);

    }
}
