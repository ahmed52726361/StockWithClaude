using Inventory.General_Interfaces;



namespace Inventory.Products
{
    public class Product : ISummarizable, IReceivesDatabaseId
    {
        public int Id { get; private set; }

        public string Name { get; private set; }

        public decimal Price { get; private set; }

        public int StockQty { get; private set; }

        public Product(string name, decimal price, int quantity)
        {
            Name = ValidateName(name);
            Price = ValidatePrice(price);

            if (quantity < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    "Stock cannot be negative.");
            }

            StockQty = quantity;
        }

        public void SetId(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(id), "ID must be positive.");
            }

            if (Id != 0)
            {
                throw new InvalidOperationException(
                    "The product already has an ID.");
            }

            Id = id;
        }

        public void Rename(string name)
        {
            Name = ValidateName(name);
        }

        public void ChangePrice(decimal price)
        {
            Price = ValidatePrice(price);
        }

        public void AddStock(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    "Quantity must be positive.");
            }

            StockQty = checked(StockQty + quantity);
        }

        public void Sale(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    "Quantity must be positive.");
            }

            if (quantity > StockQty)
            {
                throw new InvalidOperationException(
                    "Insufficient stock.");
            }

            StockQty -= quantity;
        }

        public string GetSummary()
        {
            return $"{Name} has {StockQty} in stock.";
        }

        private static string ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "Product name cannot be empty.",
                    nameof(name));
            }

            return name.Trim();
        }

        private static decimal ValidatePrice(decimal price)
        {
            if (price <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(price),
                    "Product price must be positive.");
            }

            return price;
        }
    }
}
