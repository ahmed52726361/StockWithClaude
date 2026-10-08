using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Payements
{
    internal class CashPayment : Payment
    {
        public CashPayment(decimal amount, DateTime date) : base(amount, date)
        {
            
        }

         public override string Pay()
        {
            return ProcessPayment($"Cash received: {this.Amount}");
        }
    }
}
