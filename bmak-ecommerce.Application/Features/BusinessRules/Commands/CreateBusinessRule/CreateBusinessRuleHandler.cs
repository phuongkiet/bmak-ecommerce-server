using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Entities.Rules;
using bmak_ecommerce.Domain.Interfaces;
using FluentValidation;

namespace bmak_ecommerce.Application.Features.BusinessRules.Commands.CreateBusinessRule
{
    [AutoRegister]
    public class CreateBusinessRuleHandler : ICommandHandler<CreateBusinessRuleCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateBusinessRuleCommand> _validator;

        public CreateBusinessRuleHandler(IUnitOfWork unitOfWork, IValidator<CreateBusinessRuleCommand> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<Result<int>> Handle(CreateBusinessRuleCommand command, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<int>.Failure(validationResult.Errors.First().ErrorMessage);
            }

            var entity = new BusinessRule
            {
                Name = command.Name.Trim(),
                Description = command.Description.Trim(),
                Priority = command.Priority,
                StopProcessing = command.StopProcessing,
                StartDate = command.StartDate,
                EndDate = command.EndDate,
                IsActive = command.IsActive,
                Conditions = command.Conditions.Select(x => new RuleCondition
                {
                    ConditionKey = x.ConditionKey.Trim(),
                    Operator = x.Operator,
                    ConditionValue = x.ConditionValue.Trim()
                }).ToList(),
                Actions = command.Actions.Select(x => new RuleAction
                {
                    ActionType = x.ActionType,
                    ActionValue = x.ActionValue
                }).ToList()
            };

            await _unitOfWork.Repository<BusinessRule>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(entity.Id);
        }
    }
}
