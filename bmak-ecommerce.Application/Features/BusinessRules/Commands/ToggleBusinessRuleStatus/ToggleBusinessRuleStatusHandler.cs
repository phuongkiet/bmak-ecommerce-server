using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Entities.Rules;
using bmak_ecommerce.Domain.Interfaces;
using FluentValidation;

namespace bmak_ecommerce.Application.Features.BusinessRules.Commands.ToggleBusinessRuleStatus
{
    [AutoRegister]
    public class ToggleBusinessRuleStatusHandler : ICommandHandler<ToggleBusinessRuleStatusCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<ToggleBusinessRuleStatusCommand> _validator;

        public ToggleBusinessRuleStatusHandler(IUnitOfWork unitOfWork, IValidator<ToggleBusinessRuleStatusCommand> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<Result<bool>> Handle(ToggleBusinessRuleStatusCommand command, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<bool>.Failure(validationResult.Errors.First().ErrorMessage);
            }

            var entity = await _unitOfWork.Repository<BusinessRule>().GetByIdAsync(command.Id);
            if (entity == null || entity.IsDeleted)
            {
                return Result<bool>.Failure("Không tìm thấy rule.");
            }

            entity.IsActive = command.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<BusinessRule>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
