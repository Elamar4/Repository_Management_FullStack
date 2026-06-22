using Microsoft.EntityFrameworkCore;
using Repository_management_backend.Models.Entities;
using Repository_management_backend.Models.Enums;
using Repository_management_backend.Security;

namespace Repository_management_backend.Data
{
    /// <summary>Başlanğıc seed: migrasiyaları tətbiq edir, filialları (lazımsa) və demo işçiləri
    /// (hash-lənmiş şifrə ilə) yaradır. Tətbiq HƏR işə düşəndə çağırılır (idempotent).</summary>
    public static class DbInitializer
    {
        public static void Seed(AppDbContext db, IPasswordHasher hasher)
        {
            db.Database.Migrate();

            // Filiallar — migration HasData ilə gəlir; gəlməyibsə fallback olaraq yaradılır.
            if (!db.Branches.Any())
            {
                db.Branches.AddRange(
                    new Branch { Code = "merdekan", Name = "Mərdəkan filialı", IsActive = true },
                    new Branch { Code = "pirsagi",  Name = "Pirşağı filialı",  IsActive = true },
                    new Branch { Code = "baku",     Name = "Bakı Mərkəz filialı", IsActive = true }
                );
                db.SaveChanges();
            }

            // İşçilər — runtime-da (şifrə hash olunmalıdır). BranchId koda görə tapılır.
            if (!db.Users.Any())
            {
                int Bid(string code) => db.Branches.First(b => b.Code == code).Id;

                db.Users.AddRange(
                    new User { Name = "Elmar Əliyev",     Username = "elmar",  PasswordHash = hasher.Hash("admin123"),   Role = UserRole.Admin,   BranchId = Bid("merdekan"), Phone = "+994 50 111 22 33", IsActive = true },
                    new User { Name = "Rəşad Quliyev",    Username = "rashad", PasswordHash = hasher.Hash("manager123"), Role = UserRole.Manager, BranchId = Bid("merdekan"), Phone = "+994 55 222 33 44", IsActive = true },
                    new User { Name = "Səbinə Vəliyeva",  Username = "sebine", PasswordHash = hasher.Hash("manager123"), Role = UserRole.Manager, BranchId = Bid("pirsagi"),  Phone = "+994 55 777 88 99", IsActive = true },
                    new User { Name = "Nigar Hüseynova",  Username = "nigar",  PasswordHash = hasher.Hash("user123"),    Role = UserRole.User,    BranchId = Bid("pirsagi"),  Phone = "+994 70 333 44 55", IsActive = true },
                    new User { Name = "Kamran İsmayılov", Username = "kamran", PasswordHash = hasher.Hash("user123"),    Role = UserRole.User,    BranchId = Bid("baku"),     Phone = "+994 50 222 33 44", IsActive = true }
                );
                db.SaveChanges();
            }
        }
    }
}
