namespace Repository_management_backend.Models.DTOs.Customers
{
    /// <summary>Siyahı/oxuma — borc və depozit ledger-dən hesablanır.</summary>
    public class CustomerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? ExtraPhone { get; set; }
        public string? Address { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Hesablanmış göstəricilər
        public decimal Debt { get; set; }
        public decimal Deposit { get; set; }
        public int ActiveInvoiceCount { get; set; }
    }
}
