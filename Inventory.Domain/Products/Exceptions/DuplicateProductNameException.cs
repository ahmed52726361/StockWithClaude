
namespace Inventory.Domain.Products.Exceptions;

public class DuplicateProductNameException : Exception
{
    public DuplicateProductNameException(
        string productName,
        Exception? innerException = null)
        : base(
            $"A product with the name '{productName}' already exists.",
            innerException)
    {
    }
}
