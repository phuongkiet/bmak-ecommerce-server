using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Tags.Commands.CreateTag
{
    public class CreateTagValidator : AbstractValidator<CreateTagCommand>
    {
        public CreateTagValidator()
        {
            RuleFor(t => t.Name)
                .NotEmpty().WithMessage("Tên tag không được để trống")
                .MaximumLength(255).WithMessage("Tên tag không được vượt quá 255 ký tự");

            RuleFor(t => t.Description)
                .MaximumLength(1000).WithMessage("Mô tả không được vượt quá 1000 ký tự")
                .When(t => !string.IsNullOrEmpty(t.Description));
        }
    }
}


