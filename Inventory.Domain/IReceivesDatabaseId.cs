using Inventory.Services;

namespace Inventory.General_Interfaces
{
    public interface IReceivesDatabaseId : IHasId
    {
        void SetId(int id);
    }
}
