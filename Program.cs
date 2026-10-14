// See https://aka.ms/new-console-template for more information

using Inventory;
using Inventory.Contacts;
using Inventory.DataAccess.Repositories;
using Inventory.Orders.Invoices;
using Inventory.Orders.Lines;
using Inventory.Payements;
using Inventory.Products;
using Inventory.Services;
using System.Data;



SqlProductRepository repository = new SqlProductRepository();
Product Fridge = new Product("Fridge", 10m, 7);
await repository.AddAsync(Fridge);
Console.WriteLine("Insertion completed, check database");
