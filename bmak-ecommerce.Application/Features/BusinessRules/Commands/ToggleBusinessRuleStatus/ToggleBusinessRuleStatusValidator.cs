using FluentValidation;

namespace bmak_ecommerce.Application.Features.BusinessRules.Commands.ToggleBusinessRuleStatus
{
    public class ToggleBusinessRuleStatusValidator : AbstractValidator<ToggleBusinessRuleStatusCommand>
    {
        public ToggleBusinessRuleStatusValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id rule không hợp lệ.");
        }
    }
}
