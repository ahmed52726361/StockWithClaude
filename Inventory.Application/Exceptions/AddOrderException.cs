namespace Inventory.Application.Exceptions
{
    public class AddOrderException(string productName,int saleInvoiceId):Exception($"Product or Invoice not Found")
    {

    }
}
