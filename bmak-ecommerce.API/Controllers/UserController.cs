using bmak_ecommerce.API.Extensions;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Users.Commands.CreateNewUser;
using bmak_ecommerce.Application.Features.Users.Commands.DeleteUser;
using bmak_ecommerce.Application.Features.Users.Commands.UpdateUser;
using bmak_ecommerce.Application.Features.Users.Dtos;
using bmak_ecommerce.Application.Features.Users.Queries.GetAllUsers;
using bmak_ecommerce.Application.Features.Users.Queries.GetUserDetail;
using bmak_ecommerce.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace bmak_ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseApiController
    {
        private readonly ICommandHandler<CreateNewUserCommand, int> _createUserHandler;
        private readonly ICommandHandler<UpdateUserCommand, bool> _updateUserHandler;
        private readonly ICommandHandler<DeleteUserCommand, bool> _deleteUserHandler;
        private readonly IQueryHandler<GetAllUsersQuery, PagedList<UserSummaryDto>> _getUsersHandler;
        private readonly IQueryHandler<GetUserDetailQuery, UserDto> _getUserDetailHandler;

        public UserController(
            ICommandHandler<CreateNewUserCommand, int> createUserHandler,
            ICommandHandler<UpdateUserCommand, bool> updateUserHandler,
            ICommandHandler<DeleteUserCommand, bool> deleteUserHandler,
            IQueryHandler<GetAllUsersQuery, PagedList<UserSummaryDto>> getUsersHandler,
            IQueryHandler<GetUserDetailQuery, UserDto> getUserDetailHandler)
        {
            _createUserHandler = createUserHandler;
            _updateUserHandler = updateUserHandler;
            _deleteUserHandler = deleteUserHandler;
            _getUsersHandler = getUsersHandler;
            _getUserDetailHandler = getUserDetailHandler;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedList<UserSummaryDto>>>> GetUsers([FromQuery] UserSpecParams specParams)
        {
            var query = new GetAllUsersQuery(specParams);
            var result = await _getUsersHandler.Handle(query);

            if (result.IsSuccess)
                Response.AddPaginationHeader(result.Value.PageIndex, result.Value.PageSize, result.Value.TotalCount, result.Value.TotalPages);

            return HandleResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUser(int id)
        {
            var query = new GetUserDetailQuery(id);
            var result = await _getUserDetailHandler.Handle(query);
            return HandleResult(result);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> CreateUser([FromBody] CreateNewUserCommand command)
        {
            var result = await _createUserHandler.Handle(command);
            return HandleResult(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateUser(int id, [FromBody] UpdateUserCommand command)
        {
            if (id != command.UserId) return BadRequest("ID mismatch");

            var result = await _updateUserHandler.Handle(command);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(int id, [FromQuery] bool hardDelete = false)
        {
            var command = new DeleteUserCommand(id, hardDelete);
            var result = await _deleteUserHandler.Handle(command);
            return HandleResult(result);
        }
    }
}
