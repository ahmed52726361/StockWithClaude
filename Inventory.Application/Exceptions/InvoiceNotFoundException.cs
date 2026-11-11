namespace Inventory.Application.Exceptions
{
    internal class InvoiceNotFoundException(int id) : Exception($"there is no invoice with {id} ID")
    {
    }
}
