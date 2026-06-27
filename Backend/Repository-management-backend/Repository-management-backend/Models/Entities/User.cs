using Repository_management_backend.Models.Enums;

namespace Repository_management_backend.Models.Entities
{
    /// <summary>İşçi hesabı. Hər işçi bir filiala bağlıdır.</summary>
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;       // unikal
        public string PasswordHash { get; set; } = string.Empty;   // backend-də hash olunmalı
        public UserRole Role { get; set; } = UserRole.User;
        public string? Phone { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }                   // son redaktə vaxtı
        public DateTime? LastLoginAt { get; set; }                 // son giriş vaxtı
        public DateTime? LastLogoutAt { get; set; }                // son çıxış vaxtı

        // Filial
        public int BranchId { get; set; }
        public Branch? Branch { get; set; }
    }
}
