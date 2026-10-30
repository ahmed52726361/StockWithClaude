using Inventory.General_Interfaces;

namespace Inventory.Contacts.Abstract
{
    public abstract class AbsContact
        : ISummarizable, IReceivesDatabaseId
    {
        private string _Name= string.Empty;
        private string? _Email= string.Empty;
        public string Name
        {
            get { return _Name; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Name can't be empty or null", nameof(Name));
                }
                else
                {
                    _Name = value;
                }
            }
        }
        public string? Address { get; set; }
        public string? Email
        {
            get { return _Email; }
            set
            {
                if(value is not null && (string.IsNullOrEmpty(value) || !value.Contains("@")))
                {
                    throw new ArgumentException("Email must be a valid email address", nameof(Email));
                }
                else
                {
                    _Email = value;
                }
            }
        }
        public string? PhoneNumber { get; set; }

        public int Id {  get;private set; }

        public AbsContact(string name, string? address, string? email, string? phoneNumber)
        {
              Name = name;
            Address = address;
            Email = email;
            PhoneNumber = phoneNumber;
        }
        public virtual string GetSummary()
        {
            return $"{Name} - {Email}";
        }

        public void SetId(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Id should be positive");
            }
            else if (Id > 0)
            {
                throw new InvalidOperationException(
                    "The record already has an ID.");
            }
            Id = id;

        }
    }
}
