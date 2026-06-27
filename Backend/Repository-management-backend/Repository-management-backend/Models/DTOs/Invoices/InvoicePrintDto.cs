namespace Repository_management_backend.Models.DTOs.Invoices
{
    /// <summary>Çap üçün hazır məlumat: şirkət/filial + qaimə detalları.</summary>
    public class InvoicePrintDto
    {
        public string CompanyName { get; set; } = "Kapital A.S. MMC";
        public string? BranchName { get; set; }
        public DateTime PrintedAt { get; set; } = DateTime.Now;
        public InvoiceDetailDto Invoice { get; set; } = new();
    }
}
