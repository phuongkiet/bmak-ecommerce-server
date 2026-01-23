using bmak_ecommerce.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bmak_ecommerce.Infrastructure.Persistence.Configuration
{
    public class ProductTagConfiguration : IEntityTypeConfiguration<ProductTag>
    {
        public void Configure(EntityTypeBuilder<ProductTag> builder)
        {
            builder.ToTable("ProductTag");

            builder.HasKey(x => x.Id);

            // Composite unique index để tránh duplicate Product-Tag pair
            builder.HasIndex(x => new { x.ProductId, x.TagId }).IsUnique();

            // Foreign keys
            builder.HasOne(x => x.Product)
                .WithMany(p => p.ProductTags)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa Product thì xóa luôn ProductTag

            builder.HasOne(x => x.Tag)
                .WithMany(t => t.ProductTags)
                .HasForeignKey(x => x.TagId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa Tag thì xóa luôn ProductTag
        }
    }
}


