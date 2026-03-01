using bmak_ecommerce.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bmak_ecommerce.Infrastructure.Persistence.Configuration
{
    public class ProductAttributeSelectionConfiguration : IEntityTypeConfiguration<ProductAttributeSelection>
    {
        public void Configure(EntityTypeBuilder<ProductAttributeSelection> builder)
        {
            builder.ToTable("ProductAttributeSelection");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.ProductId);
            builder.HasIndex(x => x.AttributeId);
            builder.HasIndex(x => x.AttributeValueId);

            builder.HasIndex(x => new { x.ProductId, x.AttributeId }).IsUnique();

            builder.HasOne(x => x.Product)
                .WithMany(p => p.AttributeSelections)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Attribute)
                .WithMany(a => a.ProductSelections)
                .HasForeignKey(x => x.AttributeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.AttributeValue)
                .WithMany(v => v.ProductSelections)
                .HasForeignKey(x => x.AttributeValueId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
