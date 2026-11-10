
namespace Inventory.Domain.Repository_Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        Task BeginTransactionAsync();

        Task CommitAsync();

        Task RollbackAsync();
    }
}
