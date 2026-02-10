using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Authentications.Commands.Register
{
    public class RegisterValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterValidator()
        {
            RuleFor(v => v.FullName).NotEmpty().MaximumLength(100);
            RuleFor(v => v.PhoneNumber).NotEmpty().MinimumLength(10).MaximumLength(10);
            RuleFor(v => v.Email).NotEmpty().EmailAddress();
            RuleFor(v => v.Password).NotEmpty().MinimumLength(6);
            RuleFor(v => v.ConfirmPassword)
                .Equal(v => v.Password).WithMessage("Mật khẩu nhập lại không khớp.");
        }
    }
}
