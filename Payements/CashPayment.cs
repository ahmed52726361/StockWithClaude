using Inventory.Orders.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Payements
{
    internal class CashPayment : Payment
    {
        public CashPayment(int id, IInvoice invoice, decimal amount, DateTime date) : base(id, invoice, amount, date)
        {
            
        }

         public override string Pay()
        {
            return ProcessPayment($"Cash received: {this.Amount}");
        }
    }
}
