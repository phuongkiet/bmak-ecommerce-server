using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductValidator()
        {
            RuleFor(p => p.Id)
                .GreaterThan(0).WithMessage("Product ID phải lớn hơn 0");

            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Tên sản phẩm không được để trống")
                .MaximumLength(255).WithMessage("Tên sản phẩm không được vượt quá 255 ký tự");

            RuleFor(p => p.SKU)
                .NotEmpty().WithMessage("Mã SKU là bắt buộc")
                .MinimumLength(3).WithMessage("Mã SKU phải có ít nhất 3 ký tự");

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

