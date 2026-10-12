using Inventory.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Contacts
{
    internal abstract class Contact: ISummarizable
    {
        private string _Name= string.Empty;
        private string _Email= string.Empty;
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
        public string Address { get; set; }
        public string Email
        {
            get { return _Email; }
            set
            {
                if (string.IsNullOrEmpty(value) || !value.Contains("@"))
                {
                    throw new ArgumentException("Email should not be empty or without @", nameof(Email));
                }
                else
                {
                    _Email = value;
                }
            }
        }
        public string PhoneNumber { get; set; }


        public Contact(string name, string address, string email, string phoneNumber)
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
    }
}
