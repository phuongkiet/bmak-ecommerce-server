using FluentValidation;

namespace bmak_ecommerce.Application.Features.BusinessRules.Commands.DeleteBusinessRule
{
    public class DeleteBusinessRuleValidator : AbstractValidator<DeleteBusinessRuleCommand>
    {
        public DeleteBusinessRuleValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id rule không hợp lệ.");
        }
    }
}
