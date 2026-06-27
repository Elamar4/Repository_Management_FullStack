namespace Repository_management_backend.Models.DTOs.Returns
{
    /// <summary>Tam qaytarma: bütün qaytarılan mallar geri alınır və qaimə bağlanır.</summary>
    public class FullReturnDto
    {
        public int InvoiceId { get; set; }
        public decimal RefundAmount { get; set; }
        public string? Note { get; set; }
    }
}
