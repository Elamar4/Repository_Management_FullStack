namespace Repository_management_backend.Models.Enums
{
    /// <summary>İşçi rolu (icazə səviyyəsi).</summary>
    public enum UserRole
    {
        Admin,
        Manager,
        User
    }

    /// <summary>Kateqoriya növü.</summary>
    public enum CategoryKind
    {
        Standard,   // Standart mallar (Lesa, Dəmir dirək, Taxta, ...)
        Extra,      // Əlavə kateqoriya
        Service,    // Xidmət kateqoriyası
        Pole        // Dəmir dirək ölçüsü (alt-kateqoriya)
    }

    /// <summary>İcarə tipi.</summary>
    public enum RentType
    {
        Monthly,    // Aylıq
        Daily       // Günlük
    }

    /// <summary>Ödəniş istiqaməti.</summary>
    public enum PaymentDirection
    {
        In,         // Ödəniş (daxil olan)
        Out         // Düzəliş / geri qaytarma
    }
}
