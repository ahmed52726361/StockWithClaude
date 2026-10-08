using Inventory.Orders.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Payements
{
    internal class CardPayment:Payment,IRefunded
    {
        public string Last4digits {  get; private set; }



        public CardPayment(string last4digits, int id, IInvoice invoice, decimal amount, DateTime date) : base(id, invoice, amount, date)
        {
            if (last4digits.Count()<4)
            {
                throw new ArgumentException("you should have 4 digits or more",nameof(last4digits));

            }
            else
            {
                Last4digits = last4digits;
                
            }
        }

        public override string Pay()
        {
            return ProcessPayment($"Charging card ending {this.Last4digits}: {this.Amount}");
        }
        public override string ReceiptText()
        {
            return base.ReceiptText() + $" from {Last4digits} Card";
        }

        public string Refund()
        {
            return ProcessRefund($"Card that have {Last4digits} as Last 4 digits refuned {Amount}");
        }
    }
}
