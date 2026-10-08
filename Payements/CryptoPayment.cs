using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Payements
{
    internal class CryptoPayment : Payment, IRefunded
    {

        public CryptoPayment(decimal amount, DateTime date) : base(amount, date)
        {

        }

        public override string Pay()
        {
            return ProcessPayment($"Crypto Payment received: {this.Amount}");
        }
    
        public string Refund()
        {

            return ProcessRefund($"Crypto succed to refend {Amount}");

        }
    }
}
