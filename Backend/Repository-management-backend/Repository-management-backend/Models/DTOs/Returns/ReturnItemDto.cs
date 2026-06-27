namespace Repository_management_backend.Models.DTOs.Returns
{
    /// <summary>Qismən qaytarmada bir malın qaytarılan sayı.</summary>
    public class ReturnItemDto
    {
        public int InvoiceItemId { get; set; }
        public decimal Quantity { get; set; }
    }
}
