namespace Inventory.Domain.Products.Exceptions
{
    public class InsufficientStockException(int productId,int requestedQuantity, int availableQuantity) : Exception(
        $"Insufficient stock for product ID {productId}. Requested: {requestedQuantity}, Available: {availableQuantity}.")
    {
    }
}
