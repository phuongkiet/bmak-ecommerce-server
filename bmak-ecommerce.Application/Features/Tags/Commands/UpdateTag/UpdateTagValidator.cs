using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Tags.Commands.UpdateTag
{
    public class UpdateTagValidator : AbstractValidator<UpdateTagCommand>
    {
        public UpdateTagValidator()
        {
            RuleFor(t => t.Id)
                .GreaterThan(0).WithMessage("Tag ID phải lớn hơn 0");

            RuleFor(t => t.Name)
                .NotEmpty().WithMessage("Tên tag không được để trống")
                .MaximumLength(255).WithMessage("Tên tag không được vượt quá 255 ký tự");

            RuleFor(t => t.Description)
                .MaximumLength(1000).WithMessage("Mô tả không được vượt quá 1000 ký tự")
                .When(t => !string.IsNullOrEmpty(t.Description));
        }
    }
}


