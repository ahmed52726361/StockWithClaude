using Inventory.Domain.Products;

namespace Inventory.Application
{
    public class ListProductsToSale
    {
        public int ProductID { get; set; }
        public int OrderQuantity { get; set; }
        public decimal ProductPrice { get; set; }
    }
}
