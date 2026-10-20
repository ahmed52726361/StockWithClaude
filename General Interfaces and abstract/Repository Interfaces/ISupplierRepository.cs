using Inventory.Contacts;
using Inventory.General_Interfaces.Repository_Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.General_Interfaces_and_abstract.Repository_Interfaces
{
    internal interface ISupplierRepository : IRepository<Supplier>
    {
        Task<Supplier?> FindByName(string name);
        Task<Supplier?> FindByAddress(string address);
        Task<Supplier?> FindByEmail(string email);
        Task<Supplier?> FindByPhoneNumber(string phoneNumber);

    }
}
