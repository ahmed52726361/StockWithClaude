namespace Inventory.Domain.Contacts.Interfaces
{
    public interface IPromotable
    {
        bool? AcceptsPromotions();
        string Name {  get; }   
    }
}
