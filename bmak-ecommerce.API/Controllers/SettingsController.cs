using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Settings.DTOs;
using bmak_ecommerce.Application.Features.Settings.Queries.GetAdminSetting;
using bmak_ecommerce.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bmak_ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/settings")]
    [AllowAnonymous]
    public class SettingsController : BaseApiController
    {
        private readonly IQueryHandler<GetAdminSettingQuery, AdminSettingDto> _getSettingHandler;

        public SettingsController(IQueryHandler<GetAdminSettingQuery, AdminSettingDto> getSettingHandler)
        {
            _getSettingHandler = getSettingHandler;
        }

        [HttpGet("public")]
        public async Task<ActionResult<ApiResponse<AdminSettingDto>>> GetPublicSettings()
        {
            var result = await _getSettingHandler.Handle(new GetAdminSettingQuery());
            return HandleResult(result);
        }
    }
}
