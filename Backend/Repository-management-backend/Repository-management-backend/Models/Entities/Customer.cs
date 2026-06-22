namespace Repository_management_backend.Models.Entities
{
    /// <summary>Müştəri. Filiala bağlıdır. Borc/depozit ledger-dən hesablanır.</summary>
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? ExtraPhone { get; set; }   // əlavə telefon (axtarışda iştirak edir)
        public string? Address { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }   // son redaktə vaxtı

        // Filial
        public int BranchId { get; set; }
        public Branch? Branch { get; set; }

        // Naviqasiyalar
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
        public ICollection<CustomerLedgerEntry> LedgerEntries { get; set; } = new List<CustomerLedgerEntry>();
    }
}
