using bmak_ecommerce.Application.Features.Authentications.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Authentications.Commands.Login
{
    public class LoginCommand
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
