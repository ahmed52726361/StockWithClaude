// See https://aka.ms/new-console-template for more information

using Inventory;
using Inventory.Contacts;
using Inventory.Orders.Invoices;
using Inventory.Orders.Lines;
using Inventory.Payements;
using Inventory.Services;
using System.Data;



using Inventory.DataAccess.Database;
using Microsoft.Data.SqlClient;

try
{
    using SqlConnection connection =
        SqlConnectionFactory.CreateConnection();

    connection.Open();

    Console.WriteLine("Connected successfully!");
    Console.WriteLine($"Server: {connection.DataSource}");
    Console.WriteLine($"Database: {connection.Database}");
}
catch (SqlException ex)
{
    Console.WriteLine("Database connection failed:");
    Console.WriteLine(ex.Message);
}

Console.ReadKey();


