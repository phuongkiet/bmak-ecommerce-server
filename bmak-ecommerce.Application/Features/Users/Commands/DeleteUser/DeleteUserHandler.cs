using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Users.Commands.DeleteUser
{
    [AutoRegister]

    public class DeleteUserHandler : ICommandHandler<DeleteUserCommand, bool>
    {
        private readonly IUserManagementService _userService;

        public DeleteUserHandler(IUserManagementService userService)
        {
            _userService = userService;
        }

        public async Task<Result<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            return await _userService.DeleteUserAsync(request.UserId.ToString(), request.IsHardDelete);
        }
    }
}
