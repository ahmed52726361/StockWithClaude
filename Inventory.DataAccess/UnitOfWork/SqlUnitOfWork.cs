using Inventory.DataAccess.Database;
using Inventory.Domain.Repository_Interfaces;
using Microsoft.Data.SqlClient;

namespace Inventory.DataAccess.UnitOfWork
{
    public class SqlUnitOfWork : IUnitOfWork
    {
        internal SqlConnection _connection = SqlConnectionFactory.CreateConnection();
        internal SqlTransaction _transaction;
        internal bool HasActiveTransaction {  get; set; }
        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
                throw new InvalidOperationException("Transaction has started already.");

            await _connection.OpenAsync();
            _transaction=(SqlTransaction)await _connection.BeginTransactionAsync();
             HasActiveTransaction = true;
       }

        public async Task CommitAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("Transaction has not been started.");
            await _transaction.CommitAsync();
            HasActiveTransaction= false;

        }

        public async ValueTask DisposeAsync()
        {
            if (_transaction != null)
                await _transaction.DisposeAsync();
            if (_connection != null)
                await _connection.DisposeAsync();
            HasActiveTransaction = false;
        }

        public async Task RollbackAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("Transaction has not been started.");
            await _transaction.RollbackAsync();
            HasActiveTransaction = false;
        }
    }
}
