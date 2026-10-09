using Inventory.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Contacts
{
    internal class Supplier : Contact,IHasId
    {
        public int  SupplierId { get; }
        public int Id=>SupplierId;
        public Supplier( string name, string address, string email, string phoneNumber) : base(name, address, email, phoneNumber)
        {

        }
        public override string GetSummary()
        {
            return $"Supplier Id: {SupplierId}, Name: {Name}, Email: {Email}";

        }

    }
}
