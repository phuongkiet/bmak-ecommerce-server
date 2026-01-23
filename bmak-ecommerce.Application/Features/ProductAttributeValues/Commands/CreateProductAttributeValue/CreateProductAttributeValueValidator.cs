using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.ProductAttributeValues.Commands.CreateProductAttributeValue
{
    public class CreateProductAttributeValueValidator : AbstractValidator<CreateProductAttributeValueCommand>
    {
        public CreateProductAttributeValueValidator()
        {
            RuleFor(v => v.Value)
                .NotEmpty().WithMessage("Giá trị thuộc tính không được để trống")
                .MaximumLength(255).WithMessage("Giá trị thuộc tính không được vượt quá 255 ký tự");

            RuleFor(v => v.ExtraData)
                .MaximumLength(500).WithMessage("ExtraData không được vượt quá 500 ký tự")
                .When(v => !string.IsNullOrEmpty(v.ExtraData));

            RuleFor(v => v.ProductId)
                .GreaterThan(0).WithMessage("ProductId phải lớn hơn 0");

            RuleFor(v => v.AttributeId)
                .GreaterThan(0).WithMessage("AttributeId phải lớn hơn 0");
        }
    }
}


