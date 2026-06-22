using Microsoft.EntityFrameworkCore;
using Repository_management_backend.Data;
using Repository_management_backend.Models.Entities;

namespace Repository_management_backend.Repositories
{
    /// <summary>InventoryStock və Invoice DbSet-ləri filial query filter ilə avtomatik süzülür.</summary>
    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext _db;

        public InventoryRepository(AppDbContext db) => _db = db;

        public async Task<List<InventoryStock>> GetAllAsync() =>
            await _db.InventoryStocks.AsNoTracking().OrderBy(s => s.Name).ToListAsync();

        public async Task<InventoryStock?> GetByIdAsync(int id) =>
            await _db.InventoryStocks.FirstOrDefaultAsync(s => s.Id == id);

        public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
        {
            var n = name.Trim().ToLower();
            return await _db.InventoryStocks.AnyAsync(s =>
                s.Name.ToLower() == n && (excludeId == null || s.Id != excludeId));
        }

        // Açıq qaimələrdə qaytarılan (IsReturnable) və hələ tam qaytarılmamış mallar.
        // Invoice query filter tətbiq olunduğu üçün yalnız cari filial.
        public async Task<List<RentedRow>> GetOpenRentedRowsAsync() =>
            await _db.Invoices
                .Where(i => !i.IsClosed)
                .SelectMany(i => i.Items
                    .Where(it => it.IsReturnable && (it.Quantity - it.ReturnedQuantity) > 0)
                    .Select(it => new RentedRow
                    {
                        InvoiceId = i.Id,
                        InvoiceNo = i.InvoiceNo,
                        CustomerName = i.CustomerNameSnapshot,
                        Phone = i.Phone,
                        ReturnDate = i.ReturnDate,
                        Category = it.Category,
                        Size = it.Size,
                        Unit = it.Unit,
                        Remaining = it.Quantity - it.ReturnedQuantity
                    }))
                .ToListAsync();

        public async Task AddAsync(InventoryStock stock) => await _db.InventoryStocks.AddAsync(stock);

        public void Update(InventoryStock stock) => _db.InventoryStocks.Update(stock);

        public void Remove(InventoryStock stock) => _db.InventoryStocks.Remove(stock);

        public async Task<int> SaveChangesAsync() => await _db.SaveChangesAsync();
    }
}
