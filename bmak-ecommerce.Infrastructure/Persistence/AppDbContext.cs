using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Entities.Identity;
using bmak_ecommerce.Domain.Entities.Sales;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, int>
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Các DbSet khác giữ nguyên
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<ProductAttributeValue> ProductAttributeValues { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }
        // ...

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // BẮT BUỘC: Phải gọi base trước để Identity tự cấu hình các bảng của nó (AspNetUsers, AspNetRoles...)
            base.OnModelCreating(modelBuilder);

            // --- Cấu hình lại tên bảng Identity cho gọn (Tùy chọn) ---
            modelBuilder.Entity<AppUser>().ToTable("Users");
            modelBuilder.Entity<AppRole>().ToTable("Roles");
            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<int>>().ToTable("UserClaims");
            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<int>>().ToTable("UserRoles");
            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<int>>().ToTable("UserLogins");
            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>>().ToTable("RoleClaims");
            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<int>>().ToTable("UserTokens");

            // --- Global Configurations (như cũ) ---
            modelBuilder.HasCharSet("utf8mb4");

            // ... (Phần ApplyConfigurationsFromAssembly giữ nguyên) ...
            try
            {
                modelBuilder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());
            }
            catch (System.Reflection.ReflectionTypeLoadException ex)
            {
                var loaderExceptions = ex.LoaderExceptions;
                var types = ex.Types;
                
                // Log detailed error information
                var errorDetails = new System.Text.StringBuilder();
                errorDetails.AppendLine("ReflectionTypeLoadException occurred while loading configurations:");
                
                if (loaderExceptions != null)
                {
                    for (int i = 0; i < loaderExceptions.Length; i++)
                    {
                        if (loaderExceptions[i] != null)
                        {
                            errorDetails.AppendLine($"Type {i}: {types?[i]?.FullName ?? "Unknown"} - {loaderExceptions[i].Message}");
                            errorDetails.AppendLine($"   StackTrace: {loaderExceptions[i].StackTrace}");
                        }
                    }
                }
                
                throw new InvalidOperationException(errorDetails.ToString(), ex);
            }
        }
    }
}
