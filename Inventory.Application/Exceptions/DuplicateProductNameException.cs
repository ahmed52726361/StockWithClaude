namespace Inventory.Application.Exceptions
{
    public class DuplicateProductNameException:Exception
    {
        public DuplicateProductNameException(string name) :base($"Product '{name}' already exists.") { }
    }
}
