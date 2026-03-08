using bmak_ecommerce.Domain.Entities.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bmak_ecommerce.Infrastructure.Persistence.Configuration
{
    public class BusinessRuleConfiguration : IEntityTypeConfiguration<BusinessRule>
    {
        public void Configure(EntityTypeBuilder<BusinessRule> builder)
        {
            builder.ToTable("BusinessRules");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            builder.HasMany(x => x.Conditions)
                .WithOne(x => x.BusinessRule)
                .HasForeignKey(x => x.BusinessRuleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Actions)
                .WithOne(x => x.BusinessRule)
                .HasForeignKey(x => x.BusinessRuleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
