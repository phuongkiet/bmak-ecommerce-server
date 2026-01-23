using bmak_ecommerce.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bmak_ecommerce.Infrastructure.Persistence.Configuration
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ToTable("Tag");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Slug)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false); // Slug thường là ASCII

            builder.Property(x => x.Description)
                .HasMaxLength(1000)
                .IsRequired(false);

            // Indexes
            builder.HasIndex(x => x.Slug).IsUnique(); // Slug phải unique
            builder.HasIndex(x => x.Name);

            // Many-to-Many relationship với Product
            builder.HasMany(t => t.ProductTags)
                .WithOne(pt => pt.Tag)
                .HasForeignKey(pt => pt.TagId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa Tag thì xóa luôn ProductTag
        }
    }
}


