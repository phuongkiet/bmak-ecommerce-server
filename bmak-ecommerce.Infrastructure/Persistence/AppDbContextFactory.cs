using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace bmak_ecommerce.Infrastructure.Persistence
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // 1. Trỏ đường dẫn về thư mục chứa file appsettings.json của API
            // Cấu trúc folder: Root/bmak-ecommerce.API và Root/bmak-ecommerce.Infrastructure
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../bmak-ecommerce.API"))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            // 2. Lấy Connection String
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // 3. Tạo Builder options
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            // 4. Trả về DbContext
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
