using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;

namespace bmak_ecommerce.Application.Features.Users.Commands.RestoreUser
{
    [AutoRegister]
    public class RestoreUserHandler : ICommandHandler<RestoreUserCommand, bool>
    {
        private readonly IUserManagementService _userService;

        public RestoreUserHandler(IUserManagementService userService)
        {
            _userService = userService;
        }

        public async Task<Result<bool>> Handle(RestoreUserCommand request, CancellationToken cancellationToken = default)
        {
            return await _userService.RestoreUserAsync(request.UserId.ToString());
        }
    }
}
