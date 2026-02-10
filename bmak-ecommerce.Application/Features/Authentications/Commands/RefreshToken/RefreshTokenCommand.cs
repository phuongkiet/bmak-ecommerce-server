using bmak_ecommerce.Application.Features.Authentications.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Authentications.Commands.RefreshToken
{
    public class RefreshTokenCommand
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
