using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Infrastructure.Services
{
    [AutoRegister]

    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int UserId
        {
            get
            {
                var idClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier); // hoặc "sub"

                if (idClaim != null && int.TryParse(idClaim.Value, out int userId))
                {
                    return userId;
                }

                // --- MẸO SENIOR CHO DEV MÔI TRƯỜNG TEST ---
                // Nếu chưa login (hoặc đang test Postman không gửi Token), 
                // bạn có thể return 1 cứng ở đây để đỡ phải comment code trong Handler.
                // return 1; 

                return 0; // Hoặc throw Unauthorized tùy logic
            }
        }
    }
}
