using Inventory.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Inventory
{
    internal class Product:ISummarizable,IHasId
    {
        public int ProductId { get; }
        public int Id=>ProductId;
        private  short _StockQty; 
        public string Name {  get; set; }
        public decimal Price { get; set; }
        public short StockQty
        {
            get
            {
                return _StockQty;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(StockQty),"Stock can't be negative");
                }
                else
                {
                    _StockQty=value;
                }
            }
        }
        public Product(int id,string name, decimal price, short quantity)
        {
            ProductId = id;
            Name = name;
            Price = price;
            StockQty = quantity;
        }
        public void AddStock(short QuantityToAdd)
        {
            if (QuantityToAdd<=0)
            {
                throw new ArgumentOutOfRangeException(nameof(QuantityToAdd), "Quantity to add must be greater than zero.");
            }
            else
            {
                StockQty += QuantityToAdd;
            }
        }
        public void Sale(short QtyToSell)
        {
            if (QtyToSell<=0)
            {
                throw new ArgumentOutOfRangeException(nameof(QtyToSell), "Quantity to Sale must be greater than zero.");
            }else if (QtyToSell>StockQty)
            {
                throw new ArgumentOutOfRangeException(nameof(QtyToSell), "cannot exceed available stock");
            }
            else
            {
                StockQty -= QtyToSell;
            }
        }

        public string GetSummary()
        {
            return $"{this.Name} have {this.StockQty} in stock";

        }
    }
}
