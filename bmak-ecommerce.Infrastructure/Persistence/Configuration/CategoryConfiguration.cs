using bmak_ecommerce.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Infrastructure.Persistence.Configuration
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            // 1. Đặt tên bảng là "category" (singular, chữ thường)
            builder.ToTable("category");

            // 2. Khóa chính
            builder.HasKey(x => x.Id);

            // 3. Các thuộc tính cơ bản & Validation
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Slug)
                .HasMaxLength(255)
                .IsUnicode(false); // Slug thường không dấu

            builder.Property(x => x.Description)
                .HasMaxLength(1000); // Mô tả có thể dài

            // 4. Cấu hình Index (Tối ưu hiệu năng tìm kiếm)
            // Tìm theo Slug cực nhanh và đảm bảo Slug là duy nhất
            builder.HasIndex(x => x.Slug).IsUnique();

            // Index cho tìm kiếm theo tên
            builder.HasIndex(x => x.Name);

            // Index cho ParentId (để query danh mục con nhanh)
            builder.HasIndex(x => x.ParentId);

            // 5. Cấu hình Relationships (Quan hệ)

            // Quan hệ tự tham chiếu: Category có thể có Parent (Category cha)
            builder.HasOne(x => x.Parent)
                .WithMany(c => c.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
            // Restrict: Không cho phép xóa Category nếu đang có Category con (An toàn dữ liệu)

            // Quan hệ với Products: Một Category có nhiều Products
            // (Quan hệ này đã được cấu hình ở ProductConfiguration, nhưng khai báo lại để rõ ràng)
            builder.HasMany(x => x.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            // Restrict: Không cho phép xóa Category nếu đang có Product bên trong
        }
    }
}
