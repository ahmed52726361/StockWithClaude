using Inventory.Contacts.Abstract;
using Inventory.Contacts.Interfaces;
using Inventory.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Contacts
{
    internal class Client : AbsContact, IPromotable
    {
        public bool PromotionAccept {  get; set; }


        public Client(bool promotionAccept, string name, string address, string email, string phoneNumber) : base(name, address, email, phoneNumber)
        {
            PromotionAccept = promotionAccept;
        }
        
        public override string GetSummary()
        {
            return $"{Name} {(PromotionAccept ? "Accept Promotion" : "Reject Promotion")}";


        }
        public  bool AcceptsPromotions()
        {
            return PromotionAccept;
        }

    }
}
