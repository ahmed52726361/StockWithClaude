
using Inventory.Domain.Repository_Interfaces;
using Inventory.Domain.Products;
using Inventory.Application.Exceptions;

namespace Inventory.Application
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        /// <summary>Adds a new product</summary>
        /// <exception cref="DuplicateProductNameException">
        /// Thrown when the name already exists.
        /// </exception>
        public async Task<int> AddProductAsync(string name, decimal price, int quantity)
        {
            Product newProduct = new(name, price, quantity);

            Product? existingProduct = await _productRepository.FindByNameAsync(name);

            if (existingProduct != null)
                throw new DuplicateProductNameException(name);

            return await _productRepository.AddAsync(newProduct);
        }

        /// <summary>Searches for a product by ID.</summary>
        /// <exception cref="ProductNotFoundException">
        /// Thrown if the product is not found.
        /// </exception>
        public async Task<Product> FindProductAsync(int id)
        {
            Product? product = await _productRepository.GetByIdAsync(id);

            if (product == null)
                throw new ProductNotFoundException(id);

            return product;
        }

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Thrown when product is null.
        /// </exception>
        /// <exception cref="ProductNotFoundException">
        /// Thrown when the product does not exist in the database.
        /// </exception>
        public async Task<bool> UpdateProductAsync(Product product)
        {
            ArgumentNullException.ThrowIfNull(product);

            bool updated = await _productRepository.UpdateAsync(product);

            if (!updated)
                throw new ProductNotFoundException(product.Id);
            return updated;
        }

        /// <summary>Adds quantity to a product's stock.</summary>
        /// <exception cref="ProductNotFoundException">
        /// Thrown when the specified product cannot be found.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when quantity is less than or equal to zero.
        /// </exception>
        public async Task<bool> RestockProductAsync(int productId, int quantity)
        {
            Product product = await FindProductAsync(productId);

            product.AddStock(quantity);

            return await  UpdateProductAsync(product);
        }

        /// <summary>Reduces a product's stock.</summary>
        /// <exception cref="ProductNotFoundException">
        /// Thrown when the specified product cannot be found.
        /// </exception>
        public async Task ReduceInventoryAsync(int productId, int quantity)
        {
            Product product = await FindProductAsync(productId);

            product.ReduceStock(quantity);

            await UpdateProductAsync(product);
        }

        /// <summary>Changes a product's price.</summary>
        /// <exception cref="ProductNotFoundException">
        /// Thrown when the specified product cannot be found.
        /// </exception>
        public async Task<bool> ChangeProductPriceAsync(int productId, decimal newPrice)
        {
            Product product = await FindProductAsync(productId);

            product.ChangePrice(newPrice);

            return await UpdateProductAsync(product);
        }
    }
}
