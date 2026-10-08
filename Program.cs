// See https://aka.ms/new-console-template for more information

using Inventory;
using Inventory.Contacts;
using Inventory.Orders.Invoices;
using Inventory.Orders.Lines;
using Inventory.Payements;
using Inventory.Services;
using System.Data;

Supplier Mohamed = new Supplier(1, "Mohamed", "Sfax", "mohamed@gmail.com", "5454545");
Product Fridge = new Product(1, "Fridge", 10.500m, 5);

PurchaseInvoice invoice1 = new PurchaseInvoice(1, Mohamed, DateTime.Now);
invoice1.AddLine(new PurchaseOrderLine(1, invoice1, Fridge, 5, 100m));
invoice1.AddLine(new PurchaseOrderLine(2, invoice1, Fridge, 2, 100m));

Console.WriteLine($"Total: {invoice1.InvoiceAmount}");   // predict this before running
invoice1.MarkAsShipped();
Console.WriteLine(invoice1.CurrentStatus);
invoice1.MarkAsShipped();   // should this throw now?





