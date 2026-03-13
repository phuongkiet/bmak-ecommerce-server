using bmak_ecommerce.API.Extensions;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.BusinessRules.Commands.CreateBusinessRule;
using bmak_ecommerce.Application.Features.BusinessRules.Commands.DeleteBusinessRule;
using bmak_ecommerce.Application.Features.BusinessRules.Commands.ToggleBusinessRuleStatus;
using bmak_ecommerce.Application.Features.BusinessRules.Commands.UpdateBusinessRule;
using bmak_ecommerce.Application.Features.BusinessRules.DTOs;
using bmak_ecommerce.Application.Features.BusinessRules.Queries.GetBusinessRuleDetail;
using bmak_ecommerce.Application.Features.BusinessRules.Queries.GetBusinessRules;
using bmak_ecommerce.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bmak_ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusinessRulesController : BaseApiController
    {
        private readonly IQueryHandler<GetBusinessRulesQuery, PagedList<BusinessRuleDto>> _getBusinessRulesHandler;
        private readonly IQueryHandler<GetBusinessRuleDetailQuery, BusinessRuleDto> _getBusinessRuleDetailHandler;
        private readonly ICommandHandler<CreateBusinessRuleCommand, int> _createBusinessRuleHandler;
        private readonly ICommandHandler<UpdateBusinessRuleCommand, bool> _updateBusinessRuleHandler;
        private readonly ICommandHandler<ToggleBusinessRuleStatusCommand, bool> _toggleBusinessRuleStatusHandler;
        private readonly ICommandHandler<DeleteBusinessRuleCommand, bool> _deleteBusinessRuleHandler;

        public BusinessRulesController(
            IQueryHandler<GetBusinessRulesQuery, PagedList<BusinessRuleDto>> getBusinessRulesHandler,
            IQueryHandler<GetBusinessRuleDetailQuery, BusinessRuleDto> getBusinessRuleDetailHandler,
            ICommandHandler<CreateBusinessRuleCommand, int> createBusinessRuleHandler,
            ICommandHandler<UpdateBusinessRuleCommand, bool> updateBusinessRuleHandler,
            ICommandHandler<ToggleBusinessRuleStatusCommand, bool> toggleBusinessRuleStatusHandler,
            ICommandHandler<DeleteBusinessRuleCommand, bool> deleteBusinessRuleHandler)
        {
            _getBusinessRulesHandler = getBusinessRulesHandler;
            _getBusinessRuleDetailHandler = getBusinessRuleDetailHandler;
            _createBusinessRuleHandler = createBusinessRuleHandler;
            _updateBusinessRuleHandler = updateBusinessRuleHandler;
            _toggleBusinessRuleStatusHandler = toggleBusinessRuleStatusHandler;
            _deleteBusinessRuleHandler = deleteBusinessRuleHandler;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<PagedList<BusinessRuleDto>>>> GetBusinessRules([FromQuery] BusinessRuleSpecParams specParams)
        {
            if (!(User.Identity?.IsAuthenticated == true && User.IsInRole("Admin")))
            {
                // Public callers (checkout) are only allowed to read active rules.
                specParams.IsActive = true;
            }

            var result = await _getBusinessRulesHandler.Handle(new GetBusinessRulesQuery(specParams));

            if (result.IsSuccess && result.Value != null)
            {
                Response.AddPaginationHeader(
                    result.Value.PageIndex,
                    result.Value.PageSize,
                    result.Value.TotalCount,
                    result.Value.TotalPages
                );
            }

            return HandleResult(result);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<BusinessRuleDto>>> GetBusinessRuleDetail(int id)
        {
            var result = await _getBusinessRuleDetailHandler.Handle(new GetBusinessRuleDetailQuery(id));
            return HandleResult(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<int>>> CreateBusinessRule([FromBody] CreateBusinessRuleCommand command)
        {
            var result = await _createBusinessRuleHandler.Handle(command);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse<int>.Failure(result.Error ?? "Tạo rule thất bại."));
            }

            return CreatedAtAction(
                nameof(GetBusinessRuleDetail),
                new { id = result.Value },
                ApiResponse<int>.Success(result.Value, "Tạo rule thành công."));
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateBusinessRule(int id, [FromBody] UpdateBusinessRuleCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest(ApiResponse<bool>.Failure("ID trong URL và body không khớp"));
            }

            var result = await _updateBusinessRuleHandler.Handle(command);
            return HandleResult(result);
        }

        [HttpPatch("{id:int}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> ToggleStatus(int id, [FromBody] ToggleBusinessRuleStatusCommand command)
        {
            command.Id = id;
            var result = await _toggleBusinessRuleStatusHandler.Handle(command);
            return HandleResult(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteBusinessRule(int id)
        {
            var result = await _deleteBusinessRuleHandler.Handle(new DeleteBusinessRuleCommand { Id = id });
            return HandleResult(result);
        }
    }
}
