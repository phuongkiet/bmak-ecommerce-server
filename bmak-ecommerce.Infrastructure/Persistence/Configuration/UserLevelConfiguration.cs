using bmak_ecommerce.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bmak_ecommerce.Infrastructure.Persistence.Configuration
{
    public class UserLevelConfiguration : IEntityTypeConfiguration<UserLevel>
    {
        public void Configure(EntityTypeBuilder<UserLevel> builder)
        {
            builder.ToTable("UserLevels");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(120);

            builder.Property(x => x.Rank)
                .IsRequired();

            builder.HasIndex(x => x.Code).IsUnique();
            builder.HasIndex(x => x.Rank).IsUnique();

            builder.HasMany(x => x.Users)
                .WithOne(x => x.UserLevel)
                .HasForeignKey(x => x.UserLevelId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
