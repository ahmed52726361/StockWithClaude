using Inventory.Domain.Contacts;

namespace Inventory.Domain.Repository_Interfaces
{
    public interface IClientRepository:IRepository<Client>
    {
        Task<Client?> FindByName(string name);
        Task<Client?> FindByAddress(string address);
        Task<Client?> FindByEmail(string email);
        Task<Client?> FindByPhoneNumber(string phoneNumber);
        Task<IReadOnlyList<Client>> GetByPromotionAccept(bool promotionAccept);


    }
}
