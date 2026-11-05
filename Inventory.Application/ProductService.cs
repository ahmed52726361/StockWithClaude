using Inventory.Domain.Repository_Interfaces;
using Inventory.Domain.Products;

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
        /// <exception cref="DuplicateProductNameException">thrown when the name already exist.</exception>
        public async Task<int> AddProductAsync(string name, decimal price, int quantity)
        {
            Product NewProduct=new(name, price, quantity);
            Product? existingProduct = await _productRepository.FindByNameAsync(name);
            if (existingProduct != null)
            {
                throw new DuplicateProductNameException(name);
            }

            return await _productRepository.AddAsync(NewProduct);
        
        }

        /// <summary>search a product by ID</summary>
        /// <exception cref="ProductNotFoundException">thrown if product not found</exception>

        public async Task<Product> FindProductAsync(int id)
        {
            Product? product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                throw new ProductNotFoundException(id);
            }
            else
            {
                return product;
            }

        }

        /// <summary>Add quantity to a product stock</summary>
        /// <exception cref="ProductNotFoundException">
        /// Thrown when the specified product cannot be found.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="quantity"/> is less than or equal to zero.
        /// </exception>
        public async Task<bool> RestockProductAsync(int productId, int quantity)
        {
            Product product = await FindProductAsync(productId);
            product.AddStock(quantity);
            return await _productRepository.UpdateAsync(product);
        }

        /// <summary>change a price product</summary>
        /// <exception cref="ProductNotFoundException">
        /// Thrown when the specified product cannot be found.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="newPrice"/> is less than or equal to zero.
        /// </exception>
        public async Task<bool> ChangeProductPriceAsync(int productId, decimal newPrice)
        {

            Product product = await FindProductAsync(productId);
            product.ChangePrice(newPrice);
            return await _productRepository.UpdateAsync(product);
            
        }
    }
}
