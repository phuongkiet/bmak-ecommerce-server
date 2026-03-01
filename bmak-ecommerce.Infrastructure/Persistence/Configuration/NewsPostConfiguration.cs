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
    public class NewsPostConfiguration : IEntityTypeConfiguration<NewsPost>
    {
        public void Configure(EntityTypeBuilder<NewsPost> builder)
        {
            builder.ToTable("news_posts");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Slug).IsRequired().HasMaxLength(255);

            // Content thường rất dài nên để kiểu text/longtext dưới DB
            builder.Property(x => x.Content).HasColumnType("longtext");

            // Ràng buộc Unique cho Slug (Không cho phép 2 bài viết có cùng URL)
            builder.HasIndex(x => x.Slug).IsUnique();

            // Quan hệ với Danh mục (Category)
            builder.HasOne(x => x.Category)
                .WithMany(x => x.Posts)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Xóa danh mục thì KHÔNG xóa bài viết (hoặc đổi thành Cascade tùy logic của bạn)

            // Quan hệ với Tác giả (AppUser)
            builder.HasOne(x => x.Author)
                .WithMany() // Trong AppUser không cần khai báo List<NewsPost> cho đỡ rối
                .HasForeignKey(x => x.AuthorId)
                .OnDelete(DeleteBehavior.SetNull); // Nếu User bị xóa, bài viết vẫn giữ lại (cần đổi AuthorId thành int? nullable nếu dùng SetNull)
                                                   // HOẶC dùng .OnDelete(DeleteBehavior.Cascade) nếu muốn xóa User là mất luôn bài viết của họ.
        }
    }
}
