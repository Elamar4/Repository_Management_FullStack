using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Repository_management_backend.Data
{
    /// <summary>`dotnet ef` (design-time) üçün DbContext yaradıcısı.
    /// HttpContext olmadığı üçün cari filial = 0 (migration sxemə təsir etmir).</summary>
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(config.GetConnectionString("DefaultConnection"))
                .Options;

            return new AppDbContext(options, null);
        }
    }
}
