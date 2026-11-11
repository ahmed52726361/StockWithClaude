using Inventory.Application.Exceptions;
using Inventory.Domain.Contacts;
using Inventory.Domain.Repository_Interfaces;

namespace Inventory.Application
{
    public class ClientService
    {
        private readonly IClientRepository _clientRepository;
        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }
        public async Task<int> AddClientAsync(Client client)
        {
            if (_clientRepository.GetByIdAsync(client.Id)!=null)
            {
                throw new DuplicateClientException(client.Id);
            }
            return await _clientRepository.AddAsync(client);
        }
        public async Task<Client> FindByNameAsync(string name)
        {
            Client? client = await _clientRepository.FindByNameAsync(name);
            if (client==null)
            {
                throw new ClientNotFoundException(name);
            }
            return client;
        }
        public async Task<Client> FindByIDAsync(int id)
        {
            Client? client = await _clientRepository.GetByIdAsync(id);
            if (client==null)
            {
                throw new ClientNotFoundException(id);
            }
            return client;
        }

    }
}
