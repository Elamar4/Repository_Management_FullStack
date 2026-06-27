using Microsoft.EntityFrameworkCore;
using Repository_management_backend.Data;
using Repository_management_backend.Models.Entities;

namespace Repository_management_backend.Repositories
{
    /// <summary>Invoice DbSet filial query filter ilə avtomatik süzülür.</summary>
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly AppDbContext _db;

        public InvoiceRepository(AppDbContext db) => _db = db;

        public async Task<List<Invoice>> GetAllAsync(string? search, bool? closed)
        {
            var q = _db.Invoices.AsNoTracking().Include(i => i.Items).AsQueryable();

            if (closed.HasValue)
                q = q.Where(i => i.IsClosed == closed.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(i =>
                    i.InvoiceNo.ToLower().Contains(s) ||
                    i.CustomerNameSnapshot.ToLower().Contains(s) ||
                    (i.Phone != null && i.Phone.Contains(s)) ||
                    (i.ExtraPhone != null && i.ExtraPhone.Contains(s)));
            }

            return await q.OrderByDescending(i => i.InvoiceDate).ToListAsync();
        }

        public async Task<Invoice?> GetByIdAsync(int id) =>
            await _db.Invoices.AsNoTracking()
                .Include(i => i.Items)
                .Include(i => i.Branch)
                .FirstOrDefaultAsync(i => i.Id == id);

        public async Task<Invoice?> GetByIdTrackedAsync(int id) =>
            await _db.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == id);

        // Filial daxilində ardıcıl nömrə (mövcud ən böyük rəqəm + 1, 4 rəqəmli)
        public async Task<string> GenerateInvoiceNoAsync(int branchId)
        {
            var numbers = await _db.Invoices
                .IgnoreQueryFilters()
                .Where(i => i.BranchId == branchId)
                .Select(i => i.InvoiceNo)
                .ToListAsync();

            int max = 0;
            foreach (var n in numbers)
                if (int.TryParse(new string((n ?? "").Where(char.IsDigit).ToArray()), out var v) && v > max)
                    max = v;

            return (max + 1).ToString("D4");
        }

        public async Task<Customer?> GetCustomerAsync(int customerId) =>
            await _db.Customers.FirstOrDefaultAsync(c => c.Id == customerId);

        public async Task RemoveLedgerForInvoiceAsync(int invoiceId)
        {
            var entries = await _db.CustomerLedgerEntries
                .Where(l => l.InvoiceId == invoiceId)
                .ToListAsync();
            if (entries.Count > 0)
                _db.CustomerLedgerEntries.RemoveRange(entries);
        }

        public async Task AddLedgerAsync(CustomerLedgerEntry entry) =>
            await _db.CustomerLedgerEntries.AddAsync(entry);

        public async Task AddAsync(Invoice invoice) => await _db.Invoices.AddAsync(invoice);

        public void Update(Invoice invoice) => _db.Invoices.Update(invoice);

        public void RemoveItems(IEnumerable<InvoiceItem> items) => _db.InvoiceItems.RemoveRange(items);

        public void Remove(Invoice invoice) => _db.Invoices.Remove(invoice);

        public async Task<int> SaveChangesAsync() => await _db.SaveChangesAsync();
    }
}
