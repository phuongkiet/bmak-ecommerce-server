using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bmak_ecommerce.Domain.Entities.Media;

namespace bmak_ecommerce.Infrastructure.Persistence.Configuration
{
    public class AppImageConfiguration : IEntityTypeConfiguration<AppImage>
    {
        public void Configure(EntityTypeBuilder<AppImage> builder)
        {
            // 1. Đặt tên bảng
            builder.ToTable("AppImages");

            // 2. Khóa chính
            builder.HasKey(x => x.Id);

            // 4. Cấu hình Index (Tối ưu hiệu năng tìm kiếm)
            // Tìm theo Code cực nhanh và đảm bảo Code là duy nhất

            builder.Property(x => x.Url).HasColumnType("longtext");

            // Index cho tìm kiếm theo tên
            builder.HasIndex(x => x.Title);
        }
    }
}
