namespace Inventory.Application
{
    public class ProductNotFoundException:Exception
    {
        public ProductNotFoundException(int id):base($"Product with ID={id} not found") { }
    }
}
