using bmak_ecommerce.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Infrastructure.Persistence.Configuration
{
    public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            // 1. Tên bảng
            builder.ToTable("ProductImages");

            // 2. Khóa chính
            builder.HasKey(x => x.Id);

            // 3. Cấu hình các thuộc tính
            builder.Property(x => x.ImageUrl)
                .IsRequired()
                .HasMaxLength(500); // URL ảnh có thể dài (Cloudinary/S3)

            builder.Property(x => x.IsMain)
                .HasDefaultValue(false);

            builder.Property(x => x.SortOrder)
                .HasDefaultValue(0);

            // 4. Cấu hình quan hệ (Relationship)
            // Một ProductImage thuộc về một Product
            builder.HasOne(x => x.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            // Quan trọng: Cascade có nghĩa là khi xóa Product, 
            // DB sẽ tự động xóa luôn các ProductImage liên quan.
        }
    }
}
