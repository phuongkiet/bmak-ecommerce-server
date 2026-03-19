using bmak_ecommerce.Domain.Entities.Visualizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bmak_ecommerce.Infrastructure.Persistence.Configuration
{
    public class RoomSceneConfiguration : IEntityTypeConfiguration<RoomScene>
    {
        public void Configure(EntityTypeBuilder<RoomScene> builder)
        {
            builder.ToTable("RoomScenes");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.Slug)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.ConfigJson)
                .HasColumnType("longtext")
                .IsRequired();

            builder.Property(x => x.RoomLayerUrl)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(x => x.MattLayerUrl)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(x => x.GlossyLayerUrl)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(x => x.ThumbnailUrl)
                .HasMaxLength(1000)
                .IsRequired();

            builder.HasIndex(x => x.Slug).IsUnique();
            builder.HasIndex(x => x.Title);
        }
    }
}
