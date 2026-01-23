using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryValidator()
        {
            RuleFor(c => c.Id)
                .GreaterThan(0).WithMessage("Category ID phải lớn hơn 0");

            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Tên danh mục không được để trống")
                .MaximumLength(255).WithMessage("Tên danh mục không được vượt quá 255 ký tự");

            RuleFor(c => c.Description)
                .MaximumLength(1000).WithMessage("Mô tả không được vượt quá 1000 ký tự")
                .When(c => !string.IsNullOrEmpty(c.Description));
        }
    }
}


