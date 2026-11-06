using Inventory.Domain.Contacts;

namespace Inventory.Domain.Repository_Interfaces
{
    public interface IClientRepository:IRepository<Client>
    {
        Task<Client?> FindByNameAsync(string name);
        Task<Client?> FindByAddressAsync(string address);
        Task<Client?> FindByEmailAsync(string email);
        Task<Client?> FindByPhoneNumberAsync(string phoneNumber);
        Task<IReadOnlyList<Client>> GetByPromotionAcceptAsync(bool promotionAccept);


    }
}
