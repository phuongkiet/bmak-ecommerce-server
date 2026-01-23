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
    public class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
    {
        public void Configure(EntityTypeBuilder<ProductAttribute> builder)
        {
            // 1. Đặt tên bảng
            builder.ToTable("ProductAttribute");

            // 2. Khóa chính
            builder.HasKey(x => x.Id);

            // 3. Các thuộc tính cơ bản & Validation
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(255); // VD: "Kích thước", "Màu sắc"

            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(50); // VD: "SIZE", "COLOR"
                // Code thường là unique và uppercase

            // 4. Cấu hình Index (Tối ưu hiệu năng tìm kiếm)
            // Tìm theo Code cực nhanh và đảm bảo Code là duy nhất
            builder.HasIndex(x => x.Code).IsUnique();

            // Index cho tìm kiếm theo tên
            builder.HasIndex(x => x.Name);

            // 5. Cấu hình Relationships (Quan hệ)
            // Một ProductAttribute có nhiều ProductAttributeValue
            // (Quan hệ này sẽ được cấu hình ở ProductAttributeValueConfiguration)
        }
    }
}



