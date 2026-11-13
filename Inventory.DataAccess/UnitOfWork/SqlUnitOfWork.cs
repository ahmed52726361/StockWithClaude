using Inventory.DataAccess.Database;
using Inventory.Domain.Repository_Interfaces;
using Microsoft.Data.SqlClient;

namespace Inventory.DataAccess.UnitOfWork
{
    public class SqlUnitOfWork : IUnitOfWork
    {
        internal SqlConnection? _connection;
        internal SqlTransaction? _transaction;
        internal bool HasActiveTransaction {  get;private set; }
        public async Task BeginTransactionAsync()
        {
            _connection = SqlConnectionFactory.CreateConnection();
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
            await _connection.CloseAsync();
            _transaction=null;
            _connection=null;
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
            await _connection.CloseAsync();
            _transaction=null;
            _connection=null;
            HasActiveTransaction = false;
        }
    }
}
