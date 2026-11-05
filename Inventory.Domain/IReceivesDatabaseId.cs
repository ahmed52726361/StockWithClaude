namespace Inventory.Domain
{
    public interface IReceivesDatabaseId : IHasId
    {
        void SetId(int id);
    }
}
