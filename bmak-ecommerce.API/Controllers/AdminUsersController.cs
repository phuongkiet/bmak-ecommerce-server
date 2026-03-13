using bmak_ecommerce.API.Extensions;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Users.Commands.ChangePasswordAdmin;
using bmak_ecommerce.Application.Features.Users.Commands.CreateNewUser;
using bmak_ecommerce.Application.Features.Users.Commands.DeleteUser;
using bmak_ecommerce.Application.Features.Users.Commands.RestoreUser;
using bmak_ecommerce.Application.Features.Users.Commands.UpdateUser;
using bmak_ecommerce.Application.Features.Users.Dtos;
using bmak_ecommerce.Application.Features.Users.Queries.GetAllUsers;
using bmak_ecommerce.Application.Features.Users.Queries.GetUserDetail;
using bmak_ecommerce.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bmak_ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : BaseApiController
    {
        private readonly IQueryHandler<GetAllUsersQuery, PagedList<UserSummaryDto>> _getUsersHandler;
        private readonly IQueryHandler<GetUserDetailQuery, UserDto> _getUserDetailHandler;
        private readonly ICommandHandler<CreateNewUserCommand, int> _createUserHandler;
        private readonly ICommandHandler<UpdateUserCommand, bool> _updateUserHandler;
        private readonly ICommandHandler<DeleteUserCommand, bool> _deleteUserHandler;
        private readonly ICommandHandler<RestoreUserCommand, bool> _restoreUserHandler;
        private readonly ICommandHandler<ChangePasswordAdminCommand, bool> _changePasswordHandler;

        public AdminUsersController(
            IQueryHandler<GetAllUsersQuery, PagedList<UserSummaryDto>> getUsersHandler,
            IQueryHandler<GetUserDetailQuery, UserDto> getUserDetailHandler,
            ICommandHandler<CreateNewUserCommand, int> createUserHandler,
            ICommandHandler<UpdateUserCommand, bool> updateUserHandler,
            ICommandHandler<DeleteUserCommand, bool> deleteUserHandler,
            ICommandHandler<RestoreUserCommand, bool> restoreUserHandler,
            ICommandHandler<ChangePasswordAdminCommand, bool> changePasswordHandler)
        {
            _getUsersHandler = getUsersHandler;
            _getUserDetailHandler = getUserDetailHandler;
            _createUserHandler = createUserHandler;
            _updateUserHandler = updateUserHandler;
            _deleteUserHandler = deleteUserHandler;
            _restoreUserHandler = restoreUserHandler;
            _changePasswordHandler = changePasswordHandler;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedList<UserSummaryDto>>>> GetUsers([FromQuery] UserSpecParams specParams)
        {
            var result = await _getUsersHandler.Handle(new GetAllUsersQuery(specParams));

            if (result.IsSuccess)
                Response.AddPaginationHeader(result.Value.PageIndex, result.Value.PageSize, result.Value.TotalCount, result.Value.TotalPages);

            return HandleResult(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUser(int id)
        {
            var result = await _getUserDetailHandler.Handle(new GetUserDetailQuery(id));
            return HandleResult(result);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> CreateUser([FromBody] CreateNewUserCommand command)
        {
            var result = await _createUserHandler.Handle(command);
            return HandleResult(result);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateUser(int id, [FromBody] UpdateUserCommand command)
        {
            if (id != command.UserId) return BadRequest("ID mismatch");

            var result = await _updateUserHandler.Handle(command);
            return HandleResult(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(int id, [FromQuery] bool hardDelete = false)
        {
            var result = await _deleteUserHandler.Handle(new DeleteUserCommand(id, hardDelete));
            return HandleResult(result);
        }

        [HttpPatch("{id:int}/restore")]
        public async Task<ActionResult<ApiResponse<bool>>> RestoreUser(int id)
        {
            var result = await _restoreUserHandler.Handle(new RestoreUserCommand(id));
            return HandleResult(result);
        }

        [HttpPatch("{id:int}/change-password")]
        public async Task<ActionResult<ApiResponse<bool>>> ChangePassword(int id, [FromBody] ChangePasswordAdminCommand command)
        {
            command.UserId = id;
            var result = await _changePasswordHandler.Handle(command);
            return HandleResult(result);
        }
    }
}
