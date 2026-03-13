using FluentValidation;

namespace bmak_ecommerce.Application.Features.Categories.Admin.Commands.CreateAdminCategory
{
    public class CreateAdminCategoryValidator : AbstractValidator<CreateAdminCategoryCommand>
    {
        public CreateAdminCategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ten danh muc khong duoc de trong")
                .MaximumLength(255).WithMessage("Ten danh muc khong duoc vuot qua 255 ky tu");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Mo ta khong duoc vuot qua 1000 ky tu")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));
        }
    }
}
