namespace Repository_management_backend.Models.DTOs.Users
{
    /// <summary>İşçinin şifrəsini dəyişmək üçün.</summary>
    public class ChangePasswordDto
    {
        public int Id { get; set; }
        public string NewPassword { get; set; } = string.Empty;
    }
}
