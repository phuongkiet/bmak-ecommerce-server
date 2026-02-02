using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Pages.Commands.UpdatePage
{
    public class UpdatePageValidator : AbstractValidator<UpdatePageCommand>
    {
        public UpdatePageValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Title).NotEmpty().WithMessage("Tiêu đề không được để trống")
                .MaximumLength(200);
            RuleFor(x => x.Sections).NotEmpty().WithMessage("Nội dung không được để trống");
        }
    }
}
