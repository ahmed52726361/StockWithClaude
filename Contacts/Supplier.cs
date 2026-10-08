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
        public short SupplierId { get; }
        public int Id=>SupplierId;
        public Supplier(short supplierId, string name, string address, string email, string phoneNumber) : base(name, address, email, phoneNumber)
        {
            SupplierId = supplierId;

        }
        public override string GetSummary()
        {
            return $"Supplier Id: {SupplierId}, Name: {Name}, Email: {Email}";

        }

    }
}
