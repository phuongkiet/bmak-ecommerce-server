using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Authentications.Commands.Login
{
    public class LoginValidator : AbstractValidator<LoginCommand>
    {
        public LoginValidator()
        {
            RuleFor(v => v.Email).NotEmpty().EmailAddress().WithMessage("Email không hợp lệ.");
            RuleFor(v => v.Password).NotEmpty().WithMessage("Mật khẩu không được để trống.");
        }
    }
}
