namespace Repository_management_backend.Models.DTOs.Extensions
{
    /// <summary>Müddət artırma üçün təkrar hesablama önizləməsi.</summary>
    public class ExtensionPreviewDto
    {
        public int InvoiceId { get; set; }
        public string Mode { get; set; } = "month";
        public int Periods { get; set; }
        public decimal RecurringBase { get; set; }     // təkrarlanan malların bir dövr cəmi
        public decimal SuggestedAmount { get; set; }   // RecurringBase × faktor × Periods
        public DateTime CurrentReturnDate { get; set; }
        public DateTime NewReturnDate { get; set; }
    }
}
