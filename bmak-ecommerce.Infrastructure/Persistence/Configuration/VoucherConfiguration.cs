using bmak_ecommerce.Domain.Entities.Sales;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Infrastructure.Persistence.Configuration
{
    public class VoucherConfiguration : IEntityTypeConfiguration<Voucher>
    {
        public void Configure(EntityTypeBuilder<Voucher> builder)
        {
            builder.ToTable("vouchers");
            builder.HasKey(x => x.Id);

            // Cấu hình mã Code
            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(50); // Mã voucher không nên quá dài

            // Đảm bảo không thể tạo 2 mã Voucher giống hệt nhau
            builder.HasIndex(x => x.Code).IsUnique();

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            // Cấu hình kiểu dữ liệu tiền tệ để tránh lỗi Float/Int
            builder.Property(x => x.DiscountValue).HasColumnType("decimal(18,2)");
            builder.Property(x => x.MinOrderAmount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.MaxDiscountAmount).HasColumnType("decimal(18,2)");
        }
    }
}
