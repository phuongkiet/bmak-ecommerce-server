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
    public class ProductAttributeValueConfiguration : IEntityTypeConfiguration<ProductAttributeValue>
    {
        public void Configure(EntityTypeBuilder<ProductAttributeValue> builder)
        {
            // 1. Đặt tên bảng
            builder.ToTable("ProductAttributeValue");

            // 2. Khóa chính
            builder.HasKey(x => x.Id);

            // 3. Các thuộc tính cơ bản & Validation
            builder.Property(x => x.Value)
                .IsRequired()
                .HasMaxLength(255); // VD: "60x60", "Xám", "Matt"

            builder.Property(x => x.ExtraData)
                .HasMaxLength(500)
                .IsRequired(false); // Mã màu Hex (#FF5733), Icon URL, hoặc metadata khác - có thể null
                // Lưu ý: Nếu migration hiện tại đang set nullable: false, cần tạo migration mới để update

            // 4. Cấu hình Index (Tối ưu hiệu năng tìm kiếm)
            // Index cho ProductId (tìm tất cả attributes của một product)
            builder.HasIndex(x => x.ProductId);

            // Index cho AttributeId (tìm tất cả values của một attribute)
            builder.HasIndex(x => x.AttributeId);

            // Composite Index: Tìm nhanh giá trị attribute của một product cụ thể
            builder.HasIndex(x => new { x.ProductId, x.AttributeId });

            // 5. Cấu hình Relationships (Quan hệ)

            // Quan hệ với Product: Một ProductAttributeValue thuộc về một Product
            builder.HasOne(x => x.Product)
                .WithMany(p => p.AttributeValues)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            // Cascade: Nếu xóa Product, tự động xóa sạch các ProductAttributeValue ăn theo

            // Quan hệ với ProductAttribute: Một ProductAttributeValue thuộc về một ProductAttribute
            builder.HasOne(x => x.Attribute)
                .WithMany(a => a.Values)
                .HasForeignKey(x => x.AttributeId)
                .OnDelete(DeleteBehavior.Restrict);
            // Restrict: Không cho phép xóa ProductAttribute nếu đang có ProductAttributeValue đang dùng
        }
    }
}

