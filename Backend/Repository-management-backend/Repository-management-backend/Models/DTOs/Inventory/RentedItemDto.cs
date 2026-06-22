namespace Repository_management_backend.Models.DTOs.Inventory
{
    /// <summary>İcarədə olan mallar (açıq qaimələrdən aqreqasiya).</summary>
    public class RentedItemDto
    {
        public string Category { get; set; } = string.Empty;
        public string? Size { get; set; }
        public string? Unit { get; set; }
        public decimal TotalOut { get; set; }   // hələ qaytarılmamış ümumi say
        public int InvoiceCount { get; set; }   // neçə açıq qaimədə
    }
}
