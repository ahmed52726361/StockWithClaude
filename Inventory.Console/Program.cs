using Inventory.Application;
using Inventory.DataAccess.Repositories;

ProductService productService = new(new SqlProductRepository());
Console.WriteLine("Start Program");
try
{
    int id = await productService.AddProductAsync("HandBag", 10m, 7);

    Console.WriteLine($"Handbag added with id {id}");
}
catch (DuplicateProductNameException ex)
{
    Console.WriteLine($"ERROR: {ex.Message}");
}

catch (ArgumentException ex)
{
    Console.WriteLine($"Invalid input: {ex.Message}");
}
try
{
    int id = 5;
    int quantityToAdd = 3;
    bool result = await productService.RestockProductAsync(id, quantityToAdd);
    Console.WriteLine(result ? $"Quantity changed succefully":$"Nothing changed");
}catch(ProductNotFoundException ex)
{
    Console.WriteLine($"ERROR:{ex.Message}");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Invalid input: {ex.Message}");
}
try
{
    int id = 5;
    decimal newPrice = 19.99m;
    bool result = await productService.ChangeProductPriceAsync(id, newPrice);
    Console.WriteLine(result ? $"Price changed succefully":$"Nothing changed");
}catch(ProductNotFoundException ex)
{
    Console.WriteLine($"ERROR:{ex.Message}");
}
//why we don't be specific and make a ArgumentOutOfRangeException? and how shoud we know ?
catch (ArgumentException ex)
{
    Console.WriteLine($"Invalid input: {ex.Message}");
}



Console.ReadKey();