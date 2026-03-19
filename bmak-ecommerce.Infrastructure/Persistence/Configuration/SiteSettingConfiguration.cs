using bmak_ecommerce.Domain.Entities.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bmak_ecommerce.Infrastructure.Persistence.Configuration
{
    public class SiteSettingConfiguration : IEntityTypeConfiguration<SiteSetting>
    {
        public void Configure(EntityTypeBuilder<SiteSetting> builder)
        {
            builder.ToTable("SiteSettings");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompanyName)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.SiteName)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.Hotline)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.LogoUrl)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(x => x.TaxCode)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.BusinessAddress)
                .HasMaxLength(500)
                .IsRequired();
        }
    }
}
