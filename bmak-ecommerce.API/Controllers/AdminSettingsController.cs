using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Settings.Commands.UpsertAdminSetting;
using bmak_ecommerce.Application.Features.Settings.DTOs;
using bmak_ecommerce.Application.Features.Settings.Queries.GetAdminSetting;
using bmak_ecommerce.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bmak_ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/admin/settings")]
    [Authorize(Roles = "Admin")]
    public class AdminSettingsController : BaseApiController
    {
        private readonly IQueryHandler<GetAdminSettingQuery, AdminSettingDto> _getAdminSettingHandler;
        private readonly ICommandHandler<UpsertAdminSettingCommand, AdminSettingDto> _upsertAdminSettingHandler;

        public AdminSettingsController(
            IQueryHandler<GetAdminSettingQuery, AdminSettingDto> getAdminSettingHandler,
            ICommandHandler<UpsertAdminSettingCommand, AdminSettingDto> upsertAdminSettingHandler)
        {
            _getAdminSettingHandler = getAdminSettingHandler;
            _upsertAdminSettingHandler = upsertAdminSettingHandler;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<AdminSettingDto>>> GetSetting()
        {
            var result = await _getAdminSettingHandler.Handle(new GetAdminSettingQuery());
            return HandleResult(result);
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<AdminSettingDto>>> UpsertSetting([FromBody] UpsertAdminSettingCommand command)
        {
            var result = await _upsertAdminSettingHandler.Handle(command);
            return HandleResult(result);
        }
    }
}
