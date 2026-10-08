using Inventory.Orders.Lines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Orders
{
    internal class OrderManager
    {
        private List<PurchaseOrderLine> _orders = new List<PurchaseOrderLine>();

        public void AddOrder(PurchaseOrderLine order)
        {
            if (order==null)
            {
                throw new ArgumentNullException("Order is null");
            }
            else
            {
                _orders.Add(order);
                
            }
        }

        public PurchaseOrderLine FindOrder(Func<PurchaseOrderLine, bool> condition)
        {
            foreach (PurchaseOrderLine order in _orders)
            {
                if (condition(order))
                {
                    return order;
                }
            }
            return null;
        }
        public List<PurchaseOrderLine> FindAllOrders(Func<PurchaseOrderLine, bool> condition)
        {
            List<PurchaseOrderLine> Results = new List<PurchaseOrderLine>();
            foreach (PurchaseOrderLine order in _orders)
            {
                if (condition(order))
                {
                    Results.Add(order);
                }

            }
            return Results;
        }
    }
}