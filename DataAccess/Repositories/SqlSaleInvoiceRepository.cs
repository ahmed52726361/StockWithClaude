using Inventory.Contacts;
using Inventory.General_Interfaces_and_abstract.General_abstract;
using Inventory.General_Interfaces_and_abstract.Repository_Interfaces;
using Inventory.Orders.Invoices;
using Inventory.Orders.Lines;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.DataAccess.Repositories
{
    internal class SqlSaleInvoiceRepository : SqlRepositoryBase<SalesInvoice>, ISaleInvoiceRepository

    {
        public Task AddAsync(SalesInvoice item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<SalesInvoice>> FindByClientAddress(string address)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<SalesInvoice>> FindByClientEmail(string email)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<SalesInvoice>> FindByClientName(string name)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<SalesInvoice>> FindByClientPhoneNumber(string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<SalesInvoice>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<SalesInvoice>> GetAllInvoicesByClientID(int clientId)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<SalesInvoice>> GetAllInvoicesByDate(DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        public Task<SalesInvoice?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        protected override SalesInvoice BuildTable(SqlDataReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
