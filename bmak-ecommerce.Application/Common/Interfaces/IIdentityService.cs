using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Authentications.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<Result<AuthResponse>> LoginAsync(string email, string password);
        Task<Result<string>> RegisterAsync(string fullName, string phoneNumber, string email, string password);
        Task<Result<AuthResponse>> RefreshTokenAsync(TokenRequest request);
        Task<Result<bool>> LogoutAsync(int userId);
    }
}
