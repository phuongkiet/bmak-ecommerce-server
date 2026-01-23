using bmak_ecommerce.Domain.Entities.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Infrastructure.Persistence.Configuration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(x => x.OrderCode).IsUnique(); // Mã đơn không trùng

            // --- XỬ LÝ ENUM ---
            // Lưu Enum dưới dạng số nguyên (int) vào Database (Tiết kiệm bộ nhớ & Query nhanh)
            // Pending = 1, Confirmed = 2...
            builder.Property(x => x.Status)
                .HasConversion<int>();

            builder.Property(x => x.PaymentMethod)
                .HasConversion<int>();

            // --- XỬ LÝ TIỀN TỆ ---
            builder.Property(x => x.TotalAmount).HasPrecision(18, 2);
            builder.Property(x => x.ShippingFee).HasPrecision(18, 2);
            builder.Property(x => x.DiscountAmount).HasPrecision(18, 2);

            // --- QUAN HỆ ---
            // Order bắt buộc phải có User (Khách hàng)
            builder.HasOne(x => x.User)
                .WithMany() // User có thể có nhiều đơn, nhưng ở đây ta không cần Navigation property ngược lại trong class User (để code gọn)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Xóa User không được xóa đơn hàng (để lưu lịch sử kế toán)
        }
    }
}
