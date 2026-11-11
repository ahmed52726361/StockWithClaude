namespace Inventory.Application.Exceptions
{
    public class DuplicateSaleInvoiceID(int id):Exception($"{id} is already exist")
    {
    }
}
