using FluentValidation;

namespace bmak_ecommerce.Application.Features.NewsPosts.Commands.CreateNewsPost
{
    public class CreateNewsPostValidator : AbstractValidator<CreateNewsPostCommand>
    {
        public CreateNewsPostValidator()
        {
            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("CategoryId không hợp lệ");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Tiêu đề không được để trống")
                .MaximumLength(255).WithMessage("Tiêu đề không được vượt quá 255 ký tự");

            RuleFor(x => x.Summary)
                .MaximumLength(1000).WithMessage("Tóm tắt không được vượt quá 1000 ký tự")
                .When(x => !string.IsNullOrWhiteSpace(x.Summary));

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Nội dung không được để trống");
        }
    }
}
