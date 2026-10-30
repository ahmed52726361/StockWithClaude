using Inventory.Contacts.Abstract;
using Inventory.Contacts.Interfaces;

namespace Inventory.Contacts
{
    public class Client : AbsContact, IPromotable
    {
        public bool? PromotionAccept {  get; set; }


        public Client(bool? promotionAccept, string name, string? address, string ? email, string? phoneNumber) : base(name, address, email, phoneNumber)
        {
            PromotionAccept = promotionAccept;
        }
        
        public override string GetSummary()
        {

            return $"{Name} {PromotionAccept switch
            {
                true => "Accept Promotion",
                false => "Reject Promotion",
                null => "Unknown Promotion Status"
            }}";

        }
        public  bool? AcceptsPromotions()
        {
            return PromotionAccept;
        }

    }
}
