using bmak_ecommerce.Domain.Entities.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bmak_ecommerce.Infrastructure.Persistence.Configuration
{
    public class RuleConditionConfiguration : IEntityTypeConfiguration<RuleCondition>
    {
        public void Configure(EntityTypeBuilder<RuleCondition> builder)
        {
            builder.ToTable("RuleConditions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ConditionKey)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.ConditionValue)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.Operator)
                .HasConversion<int>();
        }
    }
}
