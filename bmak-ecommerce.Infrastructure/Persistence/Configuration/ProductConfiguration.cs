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
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            // 1. Đặt tên bảng (Tùy chọn: Để khớp với convention của team)
            builder.ToTable("Products");

            // 2. Khóa chính (BaseEntity đã có, nhưng khai báo lại cho rõ ràng cũng được)
            builder.HasKey(x => x.Id);

            // 3. Các thuộc tính cơ bản & Validation
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(255); // Giới hạn độ dài để tối ưu storage

            builder.Property(x => x.SKU)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Slug)
                .HasMaxLength(255)
                .IsUnicode(false); // Slug thường không dấu

            // 4. Cấu hình JSON cho MySQL (Quan trọng)
            // Giúp lưu SpecificationsJson dưới dạng JSON Native của MySQL
            builder.Property(x => x.SpecificationsJson)
                .HasColumnType("json")
                .HasDefaultValue("{}") // JSON rỗng mặc định
                .IsRequired(false); // Cho phép null hoặc dùng default value

            // 5. Cấu hình Tiền tệ (Decimal Precision)
            // (18, 2) nghĩa là tối đa 18 chữ số, trong đó 2 số sau dấu phẩy
            builder.Property(x => x.BasePrice).HasPrecision(18, 2);
            builder.Property(x => x.SalePrice).HasPrecision(18, 2);

            // Cấu hình các chỉ số vật lý
            builder.Property(x => x.ConversionFactor).HasDefaultValue(1); // Mặc định 1 thùng = 1 m2
            builder.Property(x => x.Weight).HasDefaultValue(0);

            // Cấu hình ngày giảm giá (optional - nullable)
            builder.Property(x => x.SaleStartDate)
                .HasColumnType("datetime(6)")
                .IsRequired(false);

            builder.Property(x => x.SaleEndDate)
                .HasColumnType("datetime(6)")
                .IsRequired(false);

            // 6. Cấu hình Index (Tối ưu hiệu năng tìm kiếm)
            // Tìm theo SKU cực nhanh (và đảm bảo SKU là duy nhất)
            builder.HasIndex(x => x.SKU).IsUnique();

            // Tìm theo Slug cực nhanh
            builder.HasIndex(x => x.Slug);

            // 7. Cấu hình Relationships (Quan hệ)

            // Một Product thuộc về một Category
            builder.HasOne(x => x.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            // Restrict: Không cho phép xóa Category nếu đang có Product bên trong (An toàn dữ liệu)

            // Một Product có nhiều Attribute Values
            builder.HasMany(x => x.AttributeValues)
                .WithOne(av => av.Product)
                .HasForeignKey(av => av.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            // Cascade: Nếu xóa Product, tự động xóa sạch các AttributeValue ăn theo (Dọn rác tự động)

            // Một Product có nhiều mức giá (TierPrices)
            builder.HasMany(x => x.TierPrices)
                .WithOne(tp => tp.Product)
                .HasForeignKey(tp => tp.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
