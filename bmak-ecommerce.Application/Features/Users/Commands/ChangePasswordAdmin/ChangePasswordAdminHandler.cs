using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;

namespace bmak_ecommerce.Application.Features.Users.Commands.ChangePasswordAdmin
{
    [AutoRegister]
    public class ChangePasswordAdminHandler : ICommandHandler<ChangePasswordAdminCommand, bool>
    {
        private readonly IUserManagementService _userService;

        public ChangePasswordAdminHandler(IUserManagementService userService)
        {
            _userService = userService;
        }

        public async Task<Result<bool>> Handle(ChangePasswordAdminCommand request, CancellationToken cancellationToken = default)
        {
            return await _userService.ChangePasswordAdminAsync(request.UserId.ToString(), request.NewPassword);
        }
    }
}
