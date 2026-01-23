using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Tên sản phẩm không được để trống")
                .MaximumLength(255);

            RuleFor(p => p.SKU)
                .NotEmpty().WithMessage("Mã SKU là bắt buộc")
                .MinimumLength(3);

            RuleFor(p => p.BasePrice)
                .GreaterThan(0).WithMessage("Giá gốc phải lớn hơn 0");

            RuleFor(p => p.SalePrice)
                .GreaterThanOrEqualTo(0).WithMessage("Giá giảm phải lớn hơn hoặc bằng 0")
                .LessThanOrEqualTo(p => p.BasePrice).WithMessage("Giá giảm không được lớn hơn giá gốc");

            RuleFor(p => p.CategoryId)
                .GreaterThan(0).WithMessage("Vui lòng chọn danh mục");

            // Validate ngày giảm giá
            RuleFor(p => p.SaleEndDate)
                .GreaterThan(p => p.SaleStartDate)
                .WithMessage("Ngày kết thúc giảm giá phải sau ngày bắt đầu")
                .When(p => p.SaleStartDate.HasValue && p.SaleEndDate.HasValue);
        }
    }
}
