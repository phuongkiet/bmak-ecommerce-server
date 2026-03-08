using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Entities.Rules;
using bmak_ecommerce.Domain.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace bmak_ecommerce.Application.Features.BusinessRules.Commands.UpdateBusinessRule
{
    [AutoRegister]
    public class UpdateBusinessRuleHandler : ICommandHandler<UpdateBusinessRuleCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<UpdateBusinessRuleCommand> _validator;

        public UpdateBusinessRuleHandler(IUnitOfWork unitOfWork, IValidator<UpdateBusinessRuleCommand> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<Result<bool>> Handle(UpdateBusinessRuleCommand command, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<bool>.Failure(validationResult.Errors.First().ErrorMessage);
            }

            var entity = await _unitOfWork.Repository<BusinessRule>()
                .GetAllAsQueryable()
                .Include(x => x.Conditions)
                .Include(x => x.Actions)
                .FirstOrDefaultAsync(x => x.Id == command.Id && !x.IsDeleted, cancellationToken);

            if (entity == null)
            {
                return Result<bool>.Failure("Không tìm thấy rule.");
            }

            var currentConditions = entity.Conditions.ToList();
            var currentActions = entity.Actions.ToList();
            _unitOfWork.Repository<RuleCondition>().RemoveRange(currentConditions);
            _unitOfWork.Repository<RuleAction>().RemoveRange(currentActions);

            entity.Name = command.Name.Trim();
            entity.Description = command.Description.Trim();
            entity.Priority = command.Priority;
            entity.StopProcessing = command.StopProcessing;
            entity.StartDate = command.StartDate;
            entity.EndDate = command.EndDate;
            entity.IsActive = command.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.Conditions = command.Conditions.Select(x => new RuleCondition
            {
                ConditionKey = x.ConditionKey.Trim(),
                Operator = x.Operator,
                ConditionValue = x.ConditionValue.Trim(),
                BusinessRuleId = entity.Id
            }).ToList();
            entity.Actions = command.Actions.Select(x => new RuleAction
            {
                ActionType = x.ActionType,
                ActionValue = x.ActionValue,
                BusinessRuleId = entity.Id
            }).ToList();

            _unitOfWork.Repository<BusinessRule>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
