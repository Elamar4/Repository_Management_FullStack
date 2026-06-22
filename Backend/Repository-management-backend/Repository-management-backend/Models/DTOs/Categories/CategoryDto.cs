namespace Repository_management_backend.Models.DTOs.Categories
{
    /// <summary>Kateqoriya oxuma görünüşü (Standard / Extra / Service / Pole).</summary>
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Kind { get; set; } = string.Empty;       // Standard / Extra / Service / Pole
        public string Name { get; set; } = string.Empty;
        public string? Info { get; set; }
        public decimal Price { get; set; }
        public string? Unit { get; set; }
        public string? Note { get; set; }
        public string RentType { get; set; } = string.Empty;   // Monthly / Daily

        public int? ParentId { get; set; }
        public string? ParentName { get; set; }
        public int ChildrenCount { get; set; }
    }
}
