using Inventory.Contacts.Abstract;
using Inventory.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Contacts
{
    internal class Supplier : AbsContact
    {
        public Supplier( string name, string address, string email, string phoneNumber) : base(name, address, email, phoneNumber)
        {

        }
        public override string GetSummary()
        {
            return $"Supplier Id: {this.Id}, Name: {Name}, Email: {Email}";

        }

    }
}
