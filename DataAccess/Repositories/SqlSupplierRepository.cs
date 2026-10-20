using Inventory.Contacts;
using Inventory.General_Interfaces_and_abstract.General_abstract;
using Inventory.General_Interfaces_and_abstract.Repository_Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.DataAccess.Repositories
{
    internal class SqlSupplierRepository :SqlRepositoryBase<Supplier>, ISupplierRepository
    {
        protected override Supplier BuildTable(SqlDataReader reader)
        {
            string Name=(string)reader["Name"];
            string Email = (string)reader["Email"];
            string Address = (string)reader["Address"];
            string PhoneNumber = (string)reader["PhoneNumber"];
            return new Supplier(Name, Email, Address, PhoneNumber);
        }
        public async Task AddAsync(Supplier item)
        {
            string query = @"Insert into Suppliers(Name,Address,Email,PhoneNumber)
                            VALUES(@Name,@Address,@Email,@PhoneNumber)";
            List<QueryParameter> parameters = new List<QueryParameter>
            {
                new QueryParameter("@Name",SqlDbType.NVarChar,item.Name),
                new QueryParameter("@Address",SqlDbType.NVarChar,item.Address),
                new QueryParameter("@Email",SqlDbType.NVarChar,item.Email),
                new QueryParameter("@PhoneNumber",SqlDbType.NVarChar,item.PhoneNumber)
            };
            await base.ExecuteAsync(query, parameters);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            string query = @"DELETE FROM Suppliers where SupplierID=@Id";
            List<QueryParameter> parameters = new List<QueryParameter>
            {
                new QueryParameter("@Id",SqlDbType.Int,id)
            };
            return await base.ExecuteAsync(query, parameters) > 0;
        }

        public async Task<Supplier?> FindByAddress(string address)
        {
            string query = @"SELECT SupplierID Name,Address,Email,PhoneNumber FROM Suppliers
                            where Address=@Address";
            List<QueryParameter> parameters = new List<QueryParameter>
            {
                new QueryParameter("@Address",SqlDbType.NVarChar,address)
            };
            return await base.QuerySingleAsync(query, parameters,BuildTable);
        }

        public async Task<Supplier?> FindByEmail(string email)
        {
            string query = @"SELECT SupplierID Name,Address,Email,PhoneNumber FROM Suppliers
                            where Email=@Email";
            List<QueryParameter> parameters = new List<QueryParameter>
            {
                new QueryParameter("@Email",SqlDbType.NVarChar,email)
            };
            return await QuerySingleAsync(query, parameters, BuildTable);
        }


        public async Task<Supplier?> FindByName(string name)
        {
            string query = @"SELECT SupplierID Name,Address,Email,PhoneNumber FROM Suppliers
                            where Name=@Name";
            List<QueryParameter> parameters = new List<QueryParameter>
            {
                new QueryParameter("@Name",SqlDbType.NVarChar,name)
            };
            return await QuerySingleAsync(query, parameters, BuildTable);
        }

        public async Task<Supplier?> FindByPhoneNumber(string phoneNumber)
        {
            string query = @"SELECT SupplierID Name,Address,Email,PhoneNumber FROM Suppliers
                            where PhoneNumber=@PhoneNumber";
            List<QueryParameter> parameters = new List<QueryParameter>
            {
                new QueryParameter("@PhoneNumber",SqlDbType.NVarChar,phoneNumber)
            };
            return await QuerySingleAsync(query, parameters, BuildTable);
        }

        public async Task<IReadOnlyList<Supplier>> GetAllAsync()
        {
            string query = @"SELECT SupplierID Name,Address,Email,PhoneNumber FROM Suppliers";
            return await base.QueryAsync(query, new List<QueryParameter>(), BuildTable);

        }

        public async Task<Supplier?> GetByIdAsync(int id)
        {
            string query = @"SELECT SupplierID Name,Address,Email,PhoneNumber FROM Suppliers
                            where SupplierID=@id ";
            List<QueryParameter> parameters = new List<QueryParameter>
            {
                new QueryParameter("@id",SqlDbType.Int,id)
            };
            return await base.QuerySingleAsync(query, parameters,BuildTable);
        }
    }
}
