using bmak_ecommerce.Domain.Entities.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bmak_ecommerce.Infrastructure.Persistence.Configuration
{
    public class RuleActionConfiguration : IEntityTypeConfiguration<RuleAction>
    {
        public void Configure(EntityTypeBuilder<RuleAction> builder)
        {
            builder.ToTable("RuleActions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActionType)
                .HasConversion<int>();

            builder.Property(x => x.ActionValue)
                .HasPrecision(18, 2);
        }
    }
}
