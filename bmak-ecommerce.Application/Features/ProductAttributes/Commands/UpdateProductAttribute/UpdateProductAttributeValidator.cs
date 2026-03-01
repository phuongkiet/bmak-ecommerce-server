using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.ProductAttributes.Commands.UpdateProductAttribute
{
    public class UpdateProductAttributeValidator : AbstractValidator<UpdateProductAttributeCommand>
    {
        public UpdateProductAttributeValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên thuộc tính không được để trống")
                .MaximumLength(255).WithMessage("Tên thuộc tính không được vượt quá 255 ký tự");
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Mã không được để trống")
                .MaximumLength(255).WithMessage("Mã không được vượt quá 255 ký tự");
        }
    }
}
