using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Authentications.DTOs
{
    public class AuthResponse
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Token { get; set; } // JWT Access Token
        public string RefreshToken { get; set; }
        public List<string> Roles { get; set; }
    }
}
