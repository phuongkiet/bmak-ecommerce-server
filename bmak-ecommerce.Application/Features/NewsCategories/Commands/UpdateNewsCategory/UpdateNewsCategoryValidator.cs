using FluentValidation;

namespace bmak_ecommerce.Application.Features.NewsCategories.Commands.UpdateNewsCategory
{
    public class UpdateNewsCategoryValidator : AbstractValidator<UpdateNewsCategoryCommand>
    {
        public UpdateNewsCategoryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id không hợp lệ");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên danh mục không được để trống")
                .MaximumLength(255).WithMessage("Tên danh mục không được vượt quá 255 ký tự");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Mô tả không được vượt quá 1000 ký tự")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));
        }
    }
}
