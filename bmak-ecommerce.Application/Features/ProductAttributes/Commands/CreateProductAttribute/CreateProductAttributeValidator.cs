using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.ProductAttributes.Commands.CreateProductAttribute
{
    public class CreateProductAttributeValidator : AbstractValidator<CreateProductAttributeCommand>
    {
        public CreateProductAttributeValidator()
        {
            RuleFor(a => a.Name)
                .NotEmpty().WithMessage("Tên thuộc tính không được để trống")
                .MaximumLength(255).WithMessage("Tên thuộc tính không được vượt quá 255 ký tự");

            RuleFor(a => a.Code)
                .NotEmpty().WithMessage("Mã Code là bắt buộc")
                .MaximumLength(50).WithMessage("Mã Code không được vượt quá 50 ký tự")
                .Matches("^[A-Z0-9_]+$").WithMessage("Mã Code chỉ được chứa chữ cái IN HOA, số và dấu gạch dưới (_)")
                // Code thường là UPPERCASE để dễ query và đảm bảo consistency
                .Must(code => code == code.ToUpperInvariant())
                .WithMessage("Mã Code phải là chữ IN HOA");
        }
    }
}


