namespace Repository_management_backend.Models.DTOs.Ledger
{
    /// <summary>Manual ledger əməliyyatı (borc əlavə/ödə, depozit əlavə/çıx).</summary>
    public class LedgerTransactionDto
    {
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public DateTime? Date { get; set; }
        public string? Note { get; set; }
    }
}
