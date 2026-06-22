namespace Repository_management_backend.Models.Entities
{
    /// <summary>Qaimə müddətinin artırılması tarixçəsi.</summary>
    public class ExtensionHistory
    {
        public int Id { get; set; }

        public int InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string? Mode { get; set; }            // "month" / "half"
        public decimal AddedAmount { get; set; }     // əlavə olunan borc
        public decimal PaidNow { get; set; }         // uzatma anında ödənilən
        public string? Note { get; set; }
    }
}
