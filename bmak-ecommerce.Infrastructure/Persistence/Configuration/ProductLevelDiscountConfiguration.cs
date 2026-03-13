using bmak_ecommerce.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bmak_ecommerce.Infrastructure.Persistence.Configuration
{
    public class ProductLevelDiscountConfiguration : IEntityTypeConfiguration<ProductLevelDiscount>
    {
        public void Configure(EntityTypeBuilder<ProductLevelDiscount> builder)
        {
            builder.ToTable("ProductLevelDiscounts");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.DiscountPercent)
                .HasPrecision(5, 2)
                .IsRequired();

            builder.HasIndex(x => new { x.ProductId, x.UserLevelId }).IsUnique();

            builder.HasOne(x => x.Product)
                .WithMany(x => x.LevelDiscounts)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.UserLevel)
                .WithMany(x => x.ProductLevelDiscounts)
                .HasForeignKey(x => x.UserLevelId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
