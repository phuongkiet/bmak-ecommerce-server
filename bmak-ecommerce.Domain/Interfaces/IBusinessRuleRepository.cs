using bmak_ecommerce.Domain.Entities.Rules;

namespace bmak_ecommerce.Domain.Interfaces
{
    public interface IBusinessRuleRepository
    {
        Task<List<BusinessRule>> GetActiveRulesAsync(CancellationToken cancellationToken = default);
    }
}
