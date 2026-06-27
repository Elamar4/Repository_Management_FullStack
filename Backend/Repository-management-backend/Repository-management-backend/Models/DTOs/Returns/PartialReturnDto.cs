namespace Repository_management_backend.Models.DTOs.Returns
{
    /// <summary>Qismən qaytarma: seçilmiş malların bir hissəsi.</summary>
    public class PartialReturnDto
    {
        public int InvoiceId { get; set; }
        public List<ReturnItemDto> Items { get; set; } = new();
        public decimal RefundAmount { get; set; }   // qaytarılan depozit (varsa)
        public string? Note { get; set; }
    }
}
