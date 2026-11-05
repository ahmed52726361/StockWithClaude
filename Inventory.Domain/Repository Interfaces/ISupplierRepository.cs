using Inventory.Domain.Contacts;

namespace Inventory.Domain.Repository_Interfaces
{
    public interface ISupplierRepository : IRepository<Supplier>
    {
        Task<Supplier?> FindByName(string name);
        Task<Supplier?> FindByAddress(string address);
        Task<Supplier?> FindByEmail(string email);
        Task<Supplier?> FindByPhoneNumber(string phoneNumber);

    }
}
