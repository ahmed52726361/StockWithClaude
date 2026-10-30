using Inventory.DataAccess.Repositories;
using Inventory.Payements.Abstract;

try
{
    SqlPaymentRepository paymentRepository = new();

    Payment? payment = await paymentRepository.GetByIdAsync(2);

    if (payment == null)
    {
        Console.WriteLine("Payment not found.");
        return;
    }

    Console.WriteLine($"Before: {payment.Status}");

    if (payment.PaidDate == null)
    {
        bool updated = await paymentRepository.MarkAsPaidAsync(
            payment.Id,
            DateTime.Today);

        Console.WriteLine($"Database updated: {updated}");
    }
    else
    {
        Console.WriteLine("Payment is already paid in the database.");
    }

    Payment? reloadedPayment =
        await paymentRepository.GetByIdAsync(payment.Id);

    Console.WriteLine($"Reloaded status: {reloadedPayment?.Status}");
    Console.WriteLine(
        $"Reloaded paid date: {reloadedPayment?.PaidDate?.ToShortDateString() ?? "Not paid"}");
}
catch (Exception ex)
{
    Console.WriteLine("TEST FAILED!");
    Console.WriteLine(ex);
}

Console.WriteLine("\nPress any key to exit.");
Console.ReadKey();