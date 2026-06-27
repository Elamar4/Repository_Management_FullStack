using Repository_management_backend.Models.DTOs.Customers;

namespace Repository_management_backend.Models.DTOs.Ledger
{
    /// <summary>Müştəri ledger-i: cari borc/depozit + tam tarixçə.</summary>
    public class CustomerLedgerDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal Debt { get; set; }       // Σ DebtChange
        public decimal Deposit { get; set; }    // Σ DepositChange
        public List<LedgerEntryDto> Entries { get; set; } = new();
    }
}
