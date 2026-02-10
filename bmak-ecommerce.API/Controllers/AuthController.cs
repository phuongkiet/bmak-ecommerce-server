using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Authentications.Commands.Login;
using bmak_ecommerce.Application.Features.Authentications.Commands.Logout;
using bmak_ecommerce.Application.Features.Authentications.Commands.RefreshToken;
using bmak_ecommerce.Application.Features.Authentications.Commands.Register;
using bmak_ecommerce.Application.Features.Authentications.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace bmak_ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseApiController
    {
        private readonly ICommandHandler<LoginCommand, AuthResponse> _loginHandler;
        private readonly ICommandHandler<RegisterCommand, string> _registerHandler;
        private readonly ICommandHandler<RefreshTokenCommand, AuthResponse> _refreshTokenHandler;
        private readonly ICommandHandler<LogoutCommand, bool> _logoutHandler;

        public AuthController(
            ICommandHandler<LoginCommand, AuthResponse> loginHandler,
            ICommandHandler<RegisterCommand, string> registerHandler,
            ICommandHandler<RefreshTokenCommand, AuthResponse> refreshTokenHandler,
            ICommandHandler<LogoutCommand, bool> logoutHandler)
        {
            _loginHandler = loginHandler;
            _registerHandler = registerHandler;
            _refreshTokenHandler = refreshTokenHandler;
            _logoutHandler = logoutHandler;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginCommand command)
        {
            // Controller không biết logic login là gì, chỉ biết gửi lệnh đi
            var result = await _loginHandler.Handle(command);
            return HandleResult(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<string>>> Register([FromBody] RegisterCommand command)
        {
            var result = await _registerHandler.Handle(command);
            return HandleResult(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            var result = await _refreshTokenHandler.Handle(command);
            return HandleResult(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> Logout()
        {
            // Lấy UserId từ Claims trong Token gửi lên
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (userId <= 0)
                return Unauthorized();

            var command = new LogoutCommand(userId);
            var result = await _logoutHandler.Handle(command);

            return HandleResult(result);
        }
    }
}
