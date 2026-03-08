using bmak_ecommerce.Application.Features.BusinessRules.Commands.CreateBusinessRule;
using FluentValidation;

namespace bmak_ecommerce.Application.Features.BusinessRules.Commands.UpdateBusinessRule
{
    public class UpdateBusinessRuleValidator : AbstractValidator<UpdateBusinessRuleCommand>
    {
        public UpdateBusinessRuleValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id rule không hợp lệ.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên rule là bắt buộc.")
                .MaximumLength(200).WithMessage("Tên rule tối đa 200 ký tự.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Mô tả tối đa 1000 ký tự.");

            RuleFor(x => x.Priority)
                .GreaterThan(0).WithMessage("Priority phải lớn hơn 0.");

            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate)
                .When(x => x.EndDate.HasValue)
                .WithMessage("EndDate phải lớn hơn hoặc bằng StartDate.");

            RuleFor(x => x.Actions)
                .NotNull().WithMessage("Danh sách action không được null.")
                .Must(x => x.Count > 0).WithMessage("Rule phải có ít nhất 1 action.");

            RuleForEach(x => x.Conditions)
                .SetValidator(new BusinessRuleConditionInputValidator());

            RuleForEach(x => x.Actions)
                .SetValidator(new BusinessRuleActionInputValidator());
        }
    }
}
