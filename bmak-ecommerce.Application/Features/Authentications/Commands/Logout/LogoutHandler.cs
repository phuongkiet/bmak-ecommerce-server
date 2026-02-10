using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Authentications.Commands.Logout
{
    [AutoRegister]

    public class LogoutHandler : ICommandHandler<LogoutCommand, bool>
    {
        private readonly IIdentityService _identityService;

        public LogoutHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            return await _identityService.LogoutAsync(request.UserId);
        }
    }
}
