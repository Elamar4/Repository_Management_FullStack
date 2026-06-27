namespace Repository_management_backend.Models.DTOs.Dashboard
{
    /// <summary>Dashboard bildirişi.</summary>
    public class NotificationDto
    {
        public string Type { get; set; } = string.Empty;   // overdue / today / soon / daily-due
        public string Message { get; set; } = string.Empty;
        public int? InvoiceId { get; set; }
        public string? InvoiceNo { get; set; }
        public string? CustomerName { get; set; }
        public DateTime Date { get; set; }
    }
}
