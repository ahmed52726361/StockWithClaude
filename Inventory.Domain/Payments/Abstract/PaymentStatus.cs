namespace Inventory.Domain.Payments.Abstract
{
    public enum PaymentStatus
    {
        Unpaid,
        Pending,
        Paid,
        Overdue
    }
}