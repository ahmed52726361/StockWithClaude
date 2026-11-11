namespace Inventory.Application.Exceptions
{
    public class ClientNotFoundException : Exception
    {
        public ClientNotFoundException(object identifier)
            : base($"Client '{identifier}' was not found.")
        {
        }
    }
}