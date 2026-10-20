
using Inventory.DataAccess.Repositories;
using Inventory.Products;
using System;
using System.Threading;
using System.Threading.Tasks;

Console.WriteLine("Program started.");
SqlProductRepository productRepository = new();
IReadOnlyList<Product>  AllProducts=await productRepository.GetAllAsync();
foreach (Product product in AllProducts)
{
    Console.WriteLine($"Id:{product.Id} Name :{product.Name} Qty:{product.StockQty} Price:{product.Price}");
}
Console.WriteLine("Program finished.");
