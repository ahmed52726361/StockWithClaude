using Inventory.Domain.Repository_Interfaces;

namespace Inventory.Application.Tests.FakeRepo
{
    public class FakeUnitOfWork : IUnitOfWork
    {
        private bool _isActive;   // same idea as SqlUnitOfWork.HasActiveTransaction

        // History for the tests to check. They are never reset.
        public bool Began { get; private set; }
        public bool Committed { get; private set; }
        public bool RolledBack { get; private set; }

        public Task BeginTransactionAsync()
        {
            if (_isActive)
                throw new InvalidOperationException("Transaction has started already.");
            _isActive = true;
            Began = true;
            return Task.CompletedTask;
        }

        public Task CommitAsync()
        {
            if (!_isActive)
                throw new InvalidOperationException("Transaction has not been started.");
            _isActive = false;   // like the real one, the transaction ends here
            Committed = true;
            return Task.CompletedTask;
        }

        public Task RollbackAsync()
        {
            if (!_isActive)
                throw new InvalidOperationException("Transaction has not been started.");
            _isActive = false;
            RolledBack = true;
            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            _isActive = false;
            return ValueTask.CompletedTask;
        }
    }
}