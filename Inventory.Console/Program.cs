using Inventory.Application;
using Inventory.DataAccess.Repositories;
using Inventory.DataAccess.UnitOfWork;
using Inventory.Domain.Products;
using Inventory.Domain.Repository_Interfaces;

class Program
{
    static async Task Main(string[] args)
    {
        SqlUnitOfWork unitOfWork = new SqlUnitOfWork();
       SqlProductRepository sqlProductRepository=new(unitOfWork);
        Product product = new("Handbag-test", 10m, 7);
        int id = await sqlProductRepository.AddAsync(product);
        if (id>0)
        {
            Console.WriteLine("Product saved");
        }
        else
        {
            Console.WriteLine("Product not saved");
        }

    }
}