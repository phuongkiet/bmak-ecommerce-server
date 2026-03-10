using Microsoft.EntityFrameworkCore;
using bmak_ecommerce.Domain.Entities.Directory;
using bmak_ecommerce.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bmak_ecommerce.Infrastructure.Persistence.Configuration
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Address");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ReceiverName)
                .IsRequired()
                .HasMaxLength(120);

            builder.Property(x => x.Phone)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.Street)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.ProvinceId)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.WardId)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.Type)
                .IsRequired();

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.ProvinceId);
            builder.HasIndex(x => x.WardId);

            builder.HasOne(x => x.User)
                .WithMany(x => x.Addresses)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Province)
                .WithMany()
                .HasForeignKey(x => x.ProvinceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Ward)
                .WithMany()
                .HasForeignKey(x => x.WardId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
