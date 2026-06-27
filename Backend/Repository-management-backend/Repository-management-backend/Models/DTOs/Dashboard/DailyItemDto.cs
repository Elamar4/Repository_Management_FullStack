namespace Repository_management_backend.Models.DTOs.Dashboard
{
    /// <summary>Günlük (icarə müddəti gündəlik) mal — açıq qaimələrdən.</summary>
    public class DailyItemDto
    {
        public int InvoiceId { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Size { get; set; }
        public DateTime? DueDate { get; set; }
        public int? DayCount { get; set; }
        public int DaysOverdue { get; set; }   // müsbət = vaxtı keçib
        public bool IsOverdue { get; set; }
    }
}
