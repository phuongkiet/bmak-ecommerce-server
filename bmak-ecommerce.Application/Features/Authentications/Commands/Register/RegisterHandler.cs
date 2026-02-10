using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Authentications.Commands.Register
{
    [AutoRegister]

    public class RegisterHandler : ICommandHandler<RegisterCommand, string>
    {
        private readonly IIdentityService _identityService;

        public RegisterHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            return await _identityService.RegisterAsync(request.FullName, request.PhoneNumber, request.Email, request.Password);
        }
    }
}
