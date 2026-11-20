using Inventory.Domain.Contacts;
using Inventory.Domain.Orders.Invoices;
using Inventory.Domain.Repository_Interfaces;

namespace Inventory.Application.Tests.FakeRepo
{
    public class FakeSaleInvoiceRepository :FakeRepositoryBase<SalesInvoice>, ISaleInvoiceRepository
    {
        public Dictionary<int, decimal> SavedTotals { get; } = new();
        protected override SalesInvoice Copy(SalesInvoice source)
        {
            SalesInvoice copy = new(
                source.Contact,
                source.InvoiceDate
                );
            copy.SetId(source.Id);
            
            return copy;
        }
        protected override void OnAdded(SalesInvoice original)
        {
            SavedTotals[original.Id] = original.InvoiceAmount;
        }

        #region InvoiceInterfaceMethods
        public Task<IReadOnlyList<SalesInvoice>> FindByClientAddressAsync(string address)
        {
            IReadOnlyList<SalesInvoice> salesInvoices = Items.Where(s => string.Equals(s.Contact.Address, address, StringComparison.OrdinalIgnoreCase))
                .Select(s=>Copy(s)).ToList();
            return Task.FromResult(salesInvoices);
                
        }

        public Task<IReadOnlyList<SalesInvoice>> FindByClientEmailAsync(string email)
        {
            IReadOnlyList<SalesInvoice> salesInvoices = Items.Where(s => string.Equals(s.Contact.Email, email, StringComparison.OrdinalIgnoreCase))
                .Select(s => Copy(s)).ToList();
            return Task.FromResult(salesInvoices);
        }

        public Task<IReadOnlyList<SalesInvoice>> FindByClientNameAsync(string name)
        {
            IReadOnlyList<SalesInvoice> salesInvoices = Items.Where(s => string.Equals(s.Contact.Name, name, StringComparison.OrdinalIgnoreCase))
                .Select(s => Copy(s)).ToList();
            return Task.FromResult(salesInvoices);
        }

        public Task<IReadOnlyList<SalesInvoice>> FindByClientPhoneNumberAsync(string phoneNumber)
        {
            IReadOnlyList<SalesInvoice> salesInvoices = Items.Where(s => string.Equals(s.Contact.PhoneNumber, phoneNumber, StringComparison.OrdinalIgnoreCase))
                .Select(s => Copy(s)).ToList();
            return Task.FromResult(salesInvoices);
        }
        public Task<IReadOnlyList<SalesInvoice>> GetAllInvoicesByClientIDAsync(int clientId)
        {
            IReadOnlyList<SalesInvoice> salesInvoices = Items.Where(s =>s.Contact.Id==clientId)
                .Select(s => Copy(s)).ToList();
            return Task.FromResult(salesInvoices);
        }

        public Task<IReadOnlyList<SalesInvoice>> GetAllInvoicesByDateAsync(DateTime dateTime)
        {
            IReadOnlyList<SalesInvoice> salesInvoices = Items.Where(s => s.InvoiceDate.Date==dateTime.Date)
                .Select(s => Copy(s)).ToList();
            return Task.FromResult(salesInvoices);
        }

        #endregion
    }
}
