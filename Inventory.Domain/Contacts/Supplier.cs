using Inventory.Contacts.Abstract;

namespace Inventory.Contacts
{
    public class Supplier : AbsContact
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
