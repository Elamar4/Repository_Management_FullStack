namespace Repository_management_backend.Models.Entities
{
    /// <summary>Qaimə malı (mal / xidmət sətri).
    /// Komponent sahələri (Lesa / Təkərli lesa / Dəmir dirək) lazım olmadıqda null qalır.</summary>
    public class InvoiceItem
    {
        public int Id { get; set; }

        public int InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }

        // Ümumi
        public string Category { get; set; } = string.Empty;   // "Lesa", "Təkərli lesa", "Dəmir dirək", ...
        public string? Label { get; set; }
        public string? VariantId { get; set; }
        public string? Size { get; set; }                      // ölçü/növ (məs. "5/15 / 3.00 m", "3.85")
        public string? Unit { get; set; }                      // ədəd / m / m² / gün / xidmət
        public decimal Quantity { get; set; }                  // günlük mal üçün = gün sayı
        public decimal CustomPrice { get; set; }
        public decimal Subtotal { get; set; }
        public string? Note { get; set; }

        public bool IsReturnable { get; set; } = true;
        public bool IsRecurring { get; set; } = true;
        public bool IsFixedFee { get; set; }
        public decimal ReturnedQuantity { get; set; }

        // Günlük mal (Təkərli lesa + günlük əlavə kateqoriyalar)
        public string? RentMode { get; set; }                  // "daily"
        public DateTime? DueDate { get; set; }
        public int? DayCount { get; set; }
        public decimal? DailyPrice { get; set; }

        // Lesa komponentləri
        public int? LesaHeadCount { get; set; }
        public decimal? LesaHeadPrice { get; set; }
        public int? LesaLongRodCount { get; set; }
        public int? LesaShortRodCount { get; set; }
        public int? LesaFreeTaxtaCount { get; set; }
        public int? LesaExtraTaxtaCount { get; set; }
        public decimal? LesaExtraTaxtaPrice { get; set; }

        // Təkərli lesa komponentləri
        public int? HeadCount { get; set; }
        public int? RodCount { get; set; }
        public int? VilkaCount { get; set; }
        public int? BoardCount { get; set; }
        public int? ExtraBoardCount { get; set; }
        public decimal? ExtraBoardPrice { get; set; }

        // Dəmir dirək
        public int? PoleCategoryId { get; set; }
        public int? PalesCount { get; set; }
    }
}
