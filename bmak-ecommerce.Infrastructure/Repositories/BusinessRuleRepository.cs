using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Domain.Entities.Rules;
using bmak_ecommerce.Domain.Interfaces;
using bmak_ecommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace bmak_ecommerce.Infrastructure.Repositories
{
    [AutoRegister]
    public class BusinessRuleRepository : IBusinessRuleRepository
    {
        private readonly AppDbContext _context;

        public BusinessRuleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<BusinessRule>> GetActiveRulesAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;

            return await _context.BusinessRules
                .Include(x => x.Conditions)
                .Include(x => x.Actions)
                .Where(x =>
                    !x.IsDeleted
                    && x.IsActive
                    && x.StartDate <= now
                    && (!x.EndDate.HasValue || x.EndDate >= now))
                .OrderBy(x => x.Priority)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }
    }
}
