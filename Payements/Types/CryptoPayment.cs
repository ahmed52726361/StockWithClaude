using Inventory.Orders.Invoices.Interfaces;
using Inventory.Payements.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Payements.Types
{
    internal class CryptoPayment : Payment, IRefunded
    {

        public CryptoPayment(IInvoice invoice, decimal amount, DateTime date) : base(invoice, amount, date)
        {

        }

        public override string Pay()
        {
            return ProcessPayment($"Crypto Payment received: {Amount}");
        }
    
        public string Refund()
        {

            return ProcessRefund($"Crypto succed to refend {Amount}");

        }
    }
}
