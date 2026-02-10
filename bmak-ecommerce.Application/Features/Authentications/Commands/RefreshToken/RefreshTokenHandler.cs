using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Authentications.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Authentications.Commands.RefreshToken
{
    [AutoRegister]

    public class RefreshTokenHandler : ICommandHandler<RefreshTokenCommand, AuthResponse>
    {
        private readonly IIdentityService _identityService;

        public RefreshTokenHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var tokenRequest = new TokenRequest
            {
                AccessToken = request.AccessToken,
                RefreshToken = request.RefreshToken
            };

            return await _identityService.RefreshTokenAsync(tokenRequest);
        }
    }
}
