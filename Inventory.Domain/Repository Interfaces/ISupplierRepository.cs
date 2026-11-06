using Inventory.Domain.Contacts;

namespace Inventory.Domain.Repository_Interfaces
{
    public interface ISupplierRepository : IRepository<Supplier>
    {
        Task<Supplier?> FindByNameAsync(string name);
        Task<Supplier?> FindByAddressAsync(string address);
        Task<Supplier?> FindByEmailAsync(string email);
        Task<Supplier?> FindByPhoneNumberAsync(string phoneNumber);

    }
}
