using FluentValidation;

namespace bmak_ecommerce.Application.Features.Categories.Admin.Commands.UpdateAdminCategory
{
    public class UpdateAdminCategoryValidator : AbstractValidator<UpdateAdminCategoryCommand>
    {
        public UpdateAdminCategoryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Category ID phai lon hon 0");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ten danh muc khong duoc de trong")
                .MaximumLength(255).WithMessage("Ten danh muc khong duoc vuot qua 255 ky tu");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Mo ta khong duoc vuot qua 1000 ky tu")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));
        }
    }
}
