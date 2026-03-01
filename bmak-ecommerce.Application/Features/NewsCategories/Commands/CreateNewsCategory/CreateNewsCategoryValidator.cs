using FluentValidation;

namespace bmak_ecommerce.Application.Features.NewsCategories.Commands.CreateNewsCategory
{
    public class CreateNewsCategoryValidator : AbstractValidator<CreateNewsCategoryCommand>
    {
        public CreateNewsCategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên danh mục không được để trống")
                .MaximumLength(255).WithMessage("Tên danh mục không được vượt quá 255 ký tự");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Mô tả không được vượt quá 1000 ký tự")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));
        }
    }
}
