using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Users.Commands.CreateNewUser
{
    [AutoRegister]

    public class CreateNewUserHandler : ICommandHandler<CreateNewUserCommand, int>
    {
        private readonly IUserManagementService _userService; // Dùng Interface đã tạo ở B2

        public CreateNewUserHandler(IUserManagementService userService)
        {
            _userService = userService;
        }

        public async Task<Result<int>> Handle(CreateNewUserCommand request, CancellationToken cancellationToken)
        {
            return await _userService.CreateUserAsync(
                request.FullName,
                request.Email,
                request.Password,
                request.PhoneNumber,
                request.Roles);
        }
    }
}
