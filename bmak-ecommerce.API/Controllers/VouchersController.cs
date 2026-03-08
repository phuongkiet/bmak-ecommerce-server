using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Vouchers.Commands.CreateVoucher;
using bmak_ecommerce.Application.Features.Vouchers.Commands.DeleteVoucher;
using bmak_ecommerce.Application.Features.Vouchers.Commands.ToggleVoucherStatus;
using bmak_ecommerce.Application.Features.Vouchers.Commands.UpdateVoucher;
using bmak_ecommerce.Application.Features.Vouchers.DTOs;
using bmak_ecommerce.Application.Features.Vouchers.Queries.ApplyVoucher;
using bmak_ecommerce.Application.Features.Vouchers.Queries.GetVoucherDetail;
using bmak_ecommerce.Application.Features.Vouchers.Queries.GetVouchers;
using bmak_ecommerce.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace bmak_ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VouchersController : BaseApiController
    {
        private readonly IQueryHandler<GetVouchersQuery, PagedList<VoucherDto>> _getVouchersHandler;
        private readonly IQueryHandler<GetVoucherDetailQuery, VoucherDto> _getVoucherDetailHandler;
        private readonly ICommandHandler<CreateVoucherCommand, int> _createVoucherHandler;
        private readonly ICommandHandler<UpdateVoucherCommand, bool> _updateVoucherHandler;
        private readonly ICommandHandler<DeleteVoucherCommand, bool> _deleteVoucherHandler;
        private readonly ICommandHandler<ToggleVoucherStatusCommand, bool> _toggleVoucherStatusHandler;
        private readonly IQueryHandler<ApplyVoucherQuery, VoucherResponseDto> _applyVoucherHandler;

        public VouchersController(
            IQueryHandler<GetVouchersQuery, PagedList<VoucherDto>> getVouchersHandler,
            IQueryHandler<GetVoucherDetailQuery, VoucherDto> getVoucherDetailHandler,
            ICommandHandler<CreateVoucherCommand, int> createVoucherHandler,
            ICommandHandler<UpdateVoucherCommand, bool> updateVoucherHandler,
            ICommandHandler<DeleteVoucherCommand, bool> deleteVoucherHandler,
            ICommandHandler<ToggleVoucherStatusCommand, bool> toggleVoucherStatusHandler,
            IQueryHandler<ApplyVoucherQuery, VoucherResponseDto> applyVoucherHandler)
        {
            _getVouchersHandler = getVouchersHandler;
            _getVoucherDetailHandler = getVoucherDetailHandler;
            _createVoucherHandler = createVoucherHandler;
            _updateVoucherHandler = updateVoucherHandler;
            _deleteVoucherHandler = deleteVoucherHandler;
            _toggleVoucherStatusHandler = toggleVoucherStatusHandler;
            _applyVoucherHandler = applyVoucherHandler;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedList<VoucherDto>>>> GetVouchers([FromQuery] VoucherSpecParams specParams)
        {
            var result = await _getVouchersHandler.Handle(new GetVouchersQuery(specParams));
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<VoucherDto>>> GetVoucherById(int id)
        {
            var result = await _getVoucherDetailHandler.Handle(new GetVoucherDetailQuery(id));
            return HandleResult(result);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> CreateVoucher([FromBody] CreateVoucherCommand command)
        {
            var result = await _createVoucherHandler.Handle(command);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse<int>.Failure(result.Error ?? "Tạo voucher thất bại."));
            }

            return CreatedAtAction(
                nameof(GetVoucherById),
                new { id = result.Value },
                ApiResponse<int>.Success(result.Value, "Tạo voucher thành công."));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateVoucher(int id, [FromBody] UpdateVoucherCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest(ApiResponse<bool>.Failure("ID trong URL và body không khớp"));
            }

            var result = await _updateVoucherHandler.Handle(command);
            return HandleResult(result);
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult<ApiResponse<bool>>> ToggleStatus(int id, [FromBody] ToggleVoucherStatusCommand command)
        {
            command.Id = id;
            var result = await _toggleVoucherStatusHandler.Handle(command);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteVoucher(int id)
        {
            var result = await _deleteVoucherHandler.Handle(new DeleteVoucherCommand { Id = id });
            return HandleResult(result);
        }

        [HttpPost("apply")]
        public async Task<ActionResult<ApiResponse<VoucherResponseDto>>> ApplyVoucher([FromBody] ApplyVoucherQuery query)
        {
            var result = await _applyVoucherHandler.Handle(query);
            return HandleResult(result);
        }
    }
}
