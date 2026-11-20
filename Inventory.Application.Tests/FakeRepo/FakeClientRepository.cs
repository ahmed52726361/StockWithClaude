using Inventory.Domain.Contacts;
using Inventory.Domain.Repository_Interfaces;

namespace Inventory.Application.Tests.FakeRepo
{
    internal class FakeClientRepository : FakeRepositoryBase<Client>, IClientRepository
    {
        protected override Client Copy(Client source)
        {
            Client clone = new(source.PromotionAccept, source.Name, source.Address, source.Email, source.PhoneNumber);
            clone.SetId(source.Id);
            return clone;
        }

        public Task<Client?> FindByNameAsync(string name)
        {
            ArgumentNullException.ThrowIfNull(name);
            Client? stored = Items.FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(stored is null ? null : Copy(stored));
        }
        public Task<Client?> FindByAddressAsync(string address)
        {
            Client? client = Items.FirstOrDefault(c => string.Equals(c.Address, address, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(client is null ? null : Copy(client));
        }

        public Task<Client?> FindByEmailAsync(string email)
        {
            Client? client = Items.FirstOrDefault(c => string.Equals(c.Email, email, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(client is null ? null : Copy(client));
        }


        public Task<Client?> FindByPhoneNumberAsync(string phoneNumber)
        {
            Client? client = Items.FirstOrDefault(c => string.Equals(c.PhoneNumber, phoneNumber, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(client is null ? null : Copy(client));
        }
        public Task<IReadOnlyList<Client>> GetByPromotionAcceptAsync(bool promotionAccept)
        {
            IReadOnlyList<Client> ClientPerPromotion = Items.Where(x => x.PromotionAccept == promotionAccept).Select(y => Copy(y)).ToList();
            return Task.FromResult(ClientPerPromotion);
        }
    }
}
