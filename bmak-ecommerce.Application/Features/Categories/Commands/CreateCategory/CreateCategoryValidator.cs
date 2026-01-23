using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Tên danh mục không được để trống")
                .MaximumLength(255).WithMessage("Tên danh mục không được vượt quá 255 ký tự");

            RuleFor(c => c.Description)
                .MaximumLength(1000).WithMessage("Mô tả không được vượt quá 1000 ký tự");

            // ParentId có thể null (danh mục gốc) hoặc > 0
            RuleFor(c => c.ParentId)
                .GreaterThan(0).When(c => c.ParentId.HasValue)
                .WithMessage("ID danh mục cha phải lớn hơn 0");
        }
    }
}



