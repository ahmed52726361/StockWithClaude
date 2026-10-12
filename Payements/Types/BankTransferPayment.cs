
using Inventory.Orders.Invoices.Interfaces;
using Inventory.Payements.Abstract;

namespace Inventory.Payements.Types
{
    internal class BankTransferPayment : Payment, IRefunded
    {
        public BankTransferPayment(
            IInvoice invoice,
            decimal amount,
            DateTime date)
            : base(invoice, amount, date)
        {
        }

        public override string Pay()
        {
            return MarkPending(
                $"Bank transfer initiated for {Amount}. Waiting for confirmation.");
        }

        public string ConfirmTransfer()
        {
            return ConfirmPendingPayment(
                $"Bank transfer confirmed: {Amount}.");
        }

        public string Refund()
        {
            return ProcessRefund(
                $"Refund initiated for {Amount}.");
        }
    }
}
