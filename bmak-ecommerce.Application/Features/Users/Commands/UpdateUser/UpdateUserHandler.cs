using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Users.Commands.UpdateUser
{
    [AutoRegister]

    public class UpdateUserHandler : ICommandHandler<UpdateUserCommand, bool>
    {
        private readonly IUserManagementService _userService;

        public UpdateUserHandler(IUserManagementService userService)
        {
            _userService = userService;
        }

        public async Task<Result<bool>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            return await _userService.UpdateUserAsync(
                request.UserId.ToString(),
                request.FullName,
                request.PhoneNumber,
                request.Roles,
                request.IsActive);
        }
    }
}
