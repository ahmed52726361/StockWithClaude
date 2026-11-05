namespace Inventory.Domain.Orders.Invoices.Interfaces
{
    public interface IInvoice
    {
        int InvoiceId { get; }
        decimal InvoiceAmount { get; }
    }
}
