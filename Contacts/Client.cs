using Inventory.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Contacts
{
    internal class Client : Contact,IHasId, IPromotable
    {
        public bool PromotionAccept {  get; set; }

        public int Id { get; private set; }

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

        public void SetId(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Id should be positive");
            }
            else if (this.Id > 0)
            {
                throw new ArgumentException("Product already added");
            }
            this.Id = id;
        }
    }
}
