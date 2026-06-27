namespace Repository_management_backend.Models.DTOs.Extensions
{
    /// <summary>Müddət artırma. NewReturnDate + AddedAmount (təkrar hesablama nəticəsi) + PaidNow.</summary>
    public class ExtendInvoiceDto
    {
        public int InvoiceId { get; set; }
        public DateTime NewReturnDate { get; set; }
        public decimal AddedAmount { get; set; }
        public decimal PaidNow { get; set; }
        public string? Mode { get; set; }        // month / half
        public string? Note { get; set; }
    }
}
