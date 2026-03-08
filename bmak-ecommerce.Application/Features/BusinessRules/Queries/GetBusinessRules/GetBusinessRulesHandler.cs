using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.BusinessRules.DTOs;
using bmak_ecommerce.Domain.Entities.Rules;
using bmak_ecommerce.Domain.Interfaces;
using bmak_ecommerce.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace bmak_ecommerce.Application.Features.BusinessRules.Queries.GetBusinessRules
{
    [AutoRegister]
    public class GetBusinessRulesHandler : IQueryHandler<GetBusinessRulesQuery, PagedList<BusinessRuleDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetBusinessRulesHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagedList<BusinessRuleDto>>> Handle(GetBusinessRulesQuery query, CancellationToken cancellationToken = default)
        {
            var spec = query.Params;
            var pageIndex = spec.PageIndex < 1 ? 1 : spec.PageIndex;
            var pageSize = spec.PageSize < 1 ? 10 : spec.PageSize;

            var baseQuery = _unitOfWork.Repository<BusinessRule>()
                .GetAllAsQueryable()
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(spec.Search))
            {
                var keyword = spec.Search.Trim().ToLowerInvariant();
                baseQuery = baseQuery.Where(x => x.Name.ToLower().Contains(keyword) || x.Description.ToLower().Contains(keyword));
            }

            if (spec.IsActive.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.IsActive == spec.IsActive.Value);
            }

            var totalCount = await baseQuery.CountAsync(cancellationToken);

            var items = await baseQuery
                .OrderBy(x => x.Priority)
                .ThenBy(x => x.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new BusinessRuleDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Priority = x.Priority,
                    StopProcessing = x.StopProcessing,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    IsActive = x.IsActive
                })
                .ToListAsync(cancellationToken);

            var pagedResult = new PagedList<BusinessRuleDto>(items, totalCount, pageIndex, pageSize);
            return Result<PagedList<BusinessRuleDto>>.Success(pagedResult);
        }
    }
}
