namespace Inventory.Contacts.Interfaces
{
    public interface IPromotable
    {
        bool? AcceptsPromotions();
        string Name {  get; }   
    }
}
