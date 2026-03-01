using bmak_ecommerce.Domain.Entities.NewFolder;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Infrastructure.Persistence.Configuration
{
    public class NewsCategoryConfiguration : IEntityTypeConfiguration<NewsCategory>
    {
        public void Configure(EntityTypeBuilder<NewsCategory> builder)
        {
            // 1. Tên bảng dưới Database
            builder.ToTable("news_categories");

            // 2. Khóa chính
            builder.HasKey(x => x.Id);

            // 3. Ràng buộc cho cột Name
            builder.Property(x => x.Name)
                .IsRequired() // Không được bỏ trống
                .HasMaxLength(255); // Giới hạn độ dài tránh phình DB

            // 4. Ràng buộc cho cột Slug (Rất quan trọng cho SEO)
            builder.Property(x => x.Slug)
                .IsRequired()
                .HasMaxLength(255);

            // Tạo Index Unique cho Slug để Database tự động chặn nếu thêm trùng URL
            builder.HasIndex(x => x.Slug)
                .IsUnique();

            // 5. Cột Description (Có thể bỏ trống)
            builder.Property(x => x.Description)
                .HasMaxLength(1000) // Giới hạn hợp lý cho mô tả ngắn
                .IsRequired(false);

            // 💡 LƯU Ý VỀ QUAN HỆ 1-N:
            // Vì chúng ta đã setup hàm .HasOne().WithMany() ở bên file NewsPostConfiguration rồi, 
            // EF Core đủ thông minh để tự động nhận diện ánh xạ 2 chiều. 
            // Do đó, ở file này bạn KHÔNG CẦN khai báo lại quan hệ nữa cho code sạch và đỡ bị lỗi lặp vòng (circular config).
        }
    }
}
