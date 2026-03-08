using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.BusinessRules.DTOs;
using bmak_ecommerce.Domain.Entities.Rules;
using bmak_ecommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bmak_ecommerce.Application.Features.BusinessRules.Queries.GetBusinessRuleDetail
{
    [AutoRegister]
    public class GetBusinessRuleDetailHandler : IQueryHandler<GetBusinessRuleDetailQuery, BusinessRuleDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetBusinessRuleDetailHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<BusinessRuleDto>> Handle(GetBusinessRuleDetailQuery query, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<BusinessRule>()
                .GetAllAsQueryable()
                .Include(x => x.Conditions)
                .Include(x => x.Actions)
                .FirstOrDefaultAsync(x => x.Id == query.Id && !x.IsDeleted, cancellationToken);

            if (entity == null)
            {
                return Result<BusinessRuleDto>.Failure("Không tìm thấy rule.");
            }

            return Result<BusinessRuleDto>.Success(new BusinessRuleDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Priority = entity.Priority,
                StopProcessing = entity.StopProcessing,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                IsActive = entity.IsActive,
                Conditions = entity.Conditions.Select(x => new BusinessRuleConditionDto
                {
                    Id = x.Id,
                    ConditionKey = x.ConditionKey,
                    Operator = x.Operator,
                    ConditionValue = x.ConditionValue
                }).ToList(),
                Actions = entity.Actions.Select(x => new BusinessRuleActionDto
                {
                    Id = x.Id,
                    ActionType = x.ActionType,
                    ActionValue = x.ActionValue
                }).ToList()
            });
        }
    }
}
