using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Repository_management_backend.Models.Entities;
using System.Security.Claims;

namespace Repository_management_backend.Data
{
    public class AppDbContext : DbContext
    {
        // Cari filial (cookie claim-dən). Branch izolyasiya query filter-ləri bunu işlədir.
        private readonly int _currentBranchId;

        public AppDbContext(DbContextOptions<AppDbContext> options,
                            IHttpContextAccessor? httpContextAccessor = null) : base(options)
        {
            var raw = httpContextAccessor?.HttpContext?.User?.FindFirst("BranchId")?.Value;
            _currentBranchId = int.TryParse(raw, out var b) ? b : 0;
        }

        public DbSet<Branch> Branches => Set<Branch>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<CustomerLedgerEntry> CustomerLedgerEntries => Set<CustomerLedgerEntry>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<InventoryStock> InventoryStocks => Set<InventoryStock>();
        public DbSet<ExtensionHistory> ExtensionHistories => Set<ExtensionHistory>();
        public DbSet<ReturnHistory> ReturnHistories => Set<ReturnHistory>();

        // Bütün decimal sahələr üçün dəqiqlik (money)
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
        }

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // ---------------- Branch ----------------
            b.Entity<Branch>(e =>
            {
                e.Property(x => x.Code).HasMaxLength(50).IsRequired();
                e.Property(x => x.Name).HasMaxLength(150).IsRequired();
                e.HasIndex(x => x.Code).IsUnique();
            });

            // ---------------- User ----------------
            b.Entity<User>(e =>
            {
                e.Property(x => x.Name).HasMaxLength(150).IsRequired();
                e.Property(x => x.Username).HasMaxLength(100).IsRequired();
                e.Property(x => x.PasswordHash).HasMaxLength(256).IsRequired();
                e.Property(x => x.Phone).HasMaxLength(50);
                e.Property(x => x.Role).HasConversion<string>().HasMaxLength(20);
                e.HasIndex(x => x.Username).IsUnique();
                e.HasOne(x => x.Branch).WithMany(x => x.Users)
                    .HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
            });

            // ---------------- Customer ----------------
            b.Entity<Customer>(e =>
            {
                e.Property(x => x.Name).HasMaxLength(200).IsRequired();
                e.Property(x => x.Phone).HasMaxLength(50);
                e.Property(x => x.ExtraPhone).HasMaxLength(50);
                e.Property(x => x.Address).HasMaxLength(400);
                e.Property(x => x.Note).HasMaxLength(1000);
                e.HasIndex(x => new { x.BranchId, x.Phone });
                e.HasOne(x => x.Branch).WithMany(x => x.Customers)
                    .HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
            });

            // ---------------- Invoice ----------------
            b.Entity<Invoice>(e =>
            {
                e.Property(x => x.InvoiceNo).HasMaxLength(60).IsRequired();
                e.Property(x => x.CustomerNameSnapshot).HasMaxLength(200);
                e.Property(x => x.Phone).HasMaxLength(50);
                e.Property(x => x.ExtraPhone).HasMaxLength(50);
                e.Property(x => x.Address).HasMaxLength(400);
                e.Property(x => x.Note).HasMaxLength(1000);
                // Filial daxilində qaimə nömrəsi unikal
                e.HasIndex(x => new { x.BranchId, x.InvoiceNo }).IsUnique();
                e.HasOne(x => x.Branch).WithMany(x => x.Invoices)
                    .HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Customer).WithMany(x => x.Invoices)
                    .HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
            });

            // ---------------- InvoiceItem ----------------
            b.Entity<InvoiceItem>(e =>
            {
                e.Property(x => x.Category).HasMaxLength(80).IsRequired();
                e.Property(x => x.Label).HasMaxLength(150);
                e.Property(x => x.VariantId).HasMaxLength(80);
                e.Property(x => x.Size).HasMaxLength(120);
                e.Property(x => x.Unit).HasMaxLength(30);
                e.Property(x => x.Note).HasMaxLength(1000);
                e.Property(x => x.RentMode).HasMaxLength(20);
                e.HasOne(x => x.Invoice).WithMany(x => x.Items)
                    .HasForeignKey(x => x.InvoiceId).OnDelete(DeleteBehavior.Cascade);
            });

            // ---------------- Payment ----------------
            b.Entity<Payment>(e =>
            {
                e.Property(x => x.Direction).HasConversion<string>().HasMaxLength(10);
                e.Property(x => x.Note).HasMaxLength(500);
                e.HasOne(x => x.Invoice).WithMany(x => x.Payments)
                    .HasForeignKey(x => x.InvoiceId).OnDelete(DeleteBehavior.Cascade);
            });

            // ---------------- CustomerLedgerEntry ----------------
            b.Entity<CustomerLedgerEntry>(e =>
            {
                e.Property(x => x.Type).HasMaxLength(80).IsRequired();
                e.Property(x => x.Note).HasMaxLength(1000);
                e.Property(x => x.Source).HasMaxLength(20);
                e.HasOne(x => x.Customer).WithMany(x => x.LedgerEntries)
                    .HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Cascade);
                // Çoxlu cascade yolundan qaçmaq üçün Invoice → Ledger Restrict
                e.HasOne(x => x.Invoice).WithMany()
                    .HasForeignKey(x => x.InvoiceId).OnDelete(DeleteBehavior.Restrict);
            });

            // ---------------- Category ----------------
            b.Entity<Category>(e =>
            {
                e.Property(x => x.Name).HasMaxLength(150).IsRequired();
                e.Property(x => x.Info).HasMaxLength(500);
                e.Property(x => x.Unit).HasMaxLength(30);
                e.Property(x => x.Note).HasMaxLength(500);
                e.Property(x => x.Kind).HasConversion<string>().HasMaxLength(20);
                e.Property(x => x.RentType).HasConversion<string>().HasMaxLength(20);
                e.HasOne(x => x.Branch).WithMany(x => x.Categories)
                    .HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Parent).WithMany(x => x.Children)
                    .HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.Restrict);
            });

            // ---------------- InventoryStock ----------------
            b.Entity<InventoryStock>(e =>
            {
                e.Property(x => x.Name).HasMaxLength(150).IsRequired();
                // Filial daxilində mal adı unikal
                e.HasIndex(x => new { x.BranchId, x.Name }).IsUnique();
                e.HasOne(x => x.Branch).WithMany(x => x.InventoryStocks)
                    .HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
            });

            // ---------------- ExtensionHistory ----------------
            b.Entity<ExtensionHistory>(e =>
            {
                e.Property(x => x.Mode).HasMaxLength(20);
                e.Property(x => x.Note).HasMaxLength(500);
                e.HasOne(x => x.Invoice).WithMany(x => x.ExtensionHistory)
                    .HasForeignKey(x => x.InvoiceId).OnDelete(DeleteBehavior.Cascade);
            });

            // ---------------- ReturnHistory ----------------
            b.Entity<ReturnHistory>(e =>
            {
                e.Property(x => x.Note).HasMaxLength(500);
                e.HasOne(x => x.Invoice).WithMany(x => x.ReturnHistory)
                    .HasForeignKey(x => x.InvoiceId).OnDelete(DeleteBehavior.Cascade);
            });

            // ---------------- BRANCH İZOLYASİYASI (Global Query Filter) ----------------
            // Hər sorğu avtomatik cari filiala görə süzülür. _currentBranchId cookie claim-dən gəlir.
            b.Entity<Customer>().HasQueryFilter(e => e.BranchId == _currentBranchId);
            b.Entity<Invoice>().HasQueryFilter(e => e.BranchId == _currentBranchId);
            b.Entity<Category>().HasQueryFilter(e => e.BranchId == _currentBranchId);
            b.Entity<InventoryStock>().HasQueryFilter(e => e.BranchId == _currentBranchId);

            // ---------------- Seed: Filiallar (migration ilə) ----------------
            b.Entity<Branch>().HasData(
                new Branch { Id = 1, Code = "merdekan", Name = "Mərdəkan filialı", IsActive = true },
                new Branch { Id = 2, Code = "pirsagi", Name = "Pirşağı filialı", IsActive = true },
                new Branch { Id = 3, Code = "baku", Name = "Bakı Mərkəz filialı", IsActive = true }
            );
        }
    }
}
