using Inventory.General_Interfaces;

namespace Inventory.Payements;

internal class PaymentType : IReceivesDatabaseId
{
    public int Id { get; private set; }
    public string PaymentTypeName { get; }

    public PaymentType(string paymentTypeName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(paymentTypeName);
        PaymentTypeName = paymentTypeName;
    }

    public void SetId(int id)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id));

        if (Id > 0)
            throw new InvalidOperationException("The payment type already has an ID.");

        Id = id;
    }
}