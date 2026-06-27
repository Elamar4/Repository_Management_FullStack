namespace Repository_management_backend.Models.DTOs.Payments
{
    /// <summary>Qaimə üzrə borc hesablaması + ödəniş tarixçəsi.</summary>
    public class InvoicePaymentSummaryDto
    {
        public int InvoiceId { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }       // Σ(In) − Σ(Out)
        public decimal RemainingDebt { get; set; }    // max(0, Total − Paid)
        public decimal DepositAmount { get; set; }
        public bool IsClosed { get; set; }
        public List<PaymentDto> Payments { get; set; } = new();
    }
}
