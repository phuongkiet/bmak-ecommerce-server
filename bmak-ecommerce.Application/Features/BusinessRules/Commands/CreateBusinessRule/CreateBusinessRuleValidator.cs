using bmak_ecommerce.Application.Features.BusinessRules.Commands.Common;
using FluentValidation;

namespace bmak_ecommerce.Application.Features.BusinessRules.Commands.CreateBusinessRule
{
    public class CreateBusinessRuleValidator : AbstractValidator<CreateBusinessRuleCommand>
    {
        public CreateBusinessRuleValidator()
        {
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

    public class BusinessRuleConditionInputValidator : AbstractValidator<BusinessRuleConditionInput>
    {
        public BusinessRuleConditionInputValidator()
        {
            RuleFor(x => x.ConditionKey)
                .NotEmpty().WithMessage("ConditionKey là bắt buộc.")
                .MaximumLength(100).WithMessage("ConditionKey tối đa 100 ký tự.");

            RuleFor(x => x.ConditionValue)
                .NotEmpty().WithMessage("ConditionValue là bắt buộc.")
                .MaximumLength(500).WithMessage("ConditionValue tối đa 500 ký tự.");
        }
    }

    public class BusinessRuleActionInputValidator : AbstractValidator<BusinessRuleActionInput>
    {
        public BusinessRuleActionInputValidator()
        {
            RuleFor(x => x.ActionValue)
                .GreaterThanOrEqualTo(0).WithMessage("ActionValue không được âm.");
        }
    }
}
