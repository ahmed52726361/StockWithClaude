using Inventory.Orders.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Payements
{
    internal class BankTransferPayment: Payment,IRefunded
    {

        public BankTransferPayment( IInvoice invoice,decimal amount, DateTime date):base(invoice,amount,date)
        {
        }

        public override string Pay()
        {
            return ProcessPayment($"Waiting for transfer confirmation: {Amount}");
        }

        public string Refund()
        {
            return ProcessRefund($"The bank succed to refend {Amount}");
        }
    }
}
