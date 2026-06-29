using Microsoft.EntityFrameworkCore;
using Repository_management_backend.Models.Entities;
using Repository_management_backend.Models.Enums;
using Repository_management_backend.Security;

namespace Repository_management_backend.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext db, IPasswordHasher hasher)
        {
            db.Database.Migrate();

            if (!db.Branches.Any())
            {
                db.Branches.AddRange(
                    new Branch { Code = "merdekan", Name = "Mərdəkan filialı", IsActive = true },
                    new Branch { Code = "pirsagi",  Name = "Pirşağı filialı",  IsActive = true },
                    new Branch { Code = "baku",     Name = "Bakı Mərkəz filialı", IsActive = true }
                );
                db.SaveChanges();
            }

            if (!db.Users.Any())
            {
                int Bid(string code) => db.Branches.First(b => b.Code == code).Id;

                db.Users.AddRange(
                    new User { Name = "Kapital MMC",Username = "admin.kapital", PasswordHash = hasher.Hash("Kapital@2026!"), Role = UserRole.Admin,   BranchId = Bid("merdekan"), Phone = "+994 50 111 22 33", IsActive = true }
                   
                );
                db.SaveChanges();
            }

            var legacyAdmin = db.Users.FirstOrDefault(u => u.Username == "elmar");
            if (legacyAdmin != null)
            {
                legacyAdmin.Username = "admin.kapital";
                legacyAdmin.PasswordHash = hasher.Hash("Kapital@2026!");
                db.SaveChanges();
            }
        }
    }
}
