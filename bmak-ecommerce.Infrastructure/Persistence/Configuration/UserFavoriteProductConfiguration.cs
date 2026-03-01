using bmak_ecommerce.Domain.Entities.Identity; // Nhớ đổi đúng namespace của bạn
using bmak_ecommerce.Domain.Entities.Catalog;  // Nhớ đổi đúng namespace của bạn
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bmak_ecommerce.Infrastructure.Data.Configurations // Đổi lại thư mục cấu hình của bạn
{
    public class UserFavoriteProductConfiguration : IEntityTypeConfiguration<UserFavoriteProduct>
    {
        public void Configure(EntityTypeBuilder<UserFavoriteProduct> builder)
        {
            // 1. Đặt tên bảng (Tùy chọn, EF Core mặc định sẽ lấy tên class có chữ s)
            builder.ToTable("user_favorite_products");

            // 2. Setup khóa chính kép (Composite Key) cực kỳ quan trọng
            builder.HasKey(x => new { x.UserId, x.ProductId });

            // 3. Quan hệ với AppUser (1 User có nhiều lượt Yêu thích)
            builder.HasOne(x => x.User)
                .WithMany(x => x.FavoriteProducts) // Bảng AppUser phải có: public ICollection<UserFavoriteProduct> FavoriteProducts { get; set; }
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa User thì tự động xóa các dòng thả tim của họ luôn cho sạch DB

            // 4. Quan hệ với Product (1 Product có thể nằm trong nhiều lượt Yêu thích)
            builder.HasOne(x => x.Product)
                .WithMany() // Để trống nghĩa là không cần reference list ngược lại bên trong class Product cho đỡ rối
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa Sản phẩm thì tự động gỡ sản phẩm đó khỏi danh sách yêu thích của mọi user

            // 5. Cấu hình cột ngày giờ (Tùy chọn nâng cao cho mượt)
            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)"); // Nếu dùng SQL Server, tự gán ngày giờ UTC lúc insert
        }
    }
}