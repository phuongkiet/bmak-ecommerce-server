using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Addresses.Commands.CreateAddress;
using bmak_ecommerce.Application.Features.Addresses.Commands.DeleteAddress;
using bmak_ecommerce.Application.Features.Addresses.Commands.UpdateAddress;
using bmak_ecommerce.Application.Features.Addresses.DTOs;
using bmak_ecommerce.Application.Features.Addresses.Queries.GetAddressDetail;
using bmak_ecommerce.Application.Features.Addresses.Queries.GetMyAddresses;
using Microsoft.AspNetCore.Mvc;

namespace bmak_ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : BaseApiController
    {
        private readonly IQueryHandler<GetMyAddressesQuery, List<AddressDto>> _getMyAddressesHandler;
        private readonly IQueryHandler<GetAddressDetailQuery, AddressDto> _getAddressDetailHandler;
        private readonly ICommandHandler<CreateAddressCommand, int> _createAddressHandler;
        private readonly ICommandHandler<UpdateAddressCommand, bool> _updateAddressHandler;
        private readonly ICommandHandler<DeleteAddressCommand, bool> _deleteAddressHandler;

        public AddressesController(
            IQueryHandler<GetMyAddressesQuery, List<AddressDto>> getMyAddressesHandler,
            IQueryHandler<GetAddressDetailQuery, AddressDto> getAddressDetailHandler,
            ICommandHandler<CreateAddressCommand, int> createAddressHandler,
            ICommandHandler<UpdateAddressCommand, bool> updateAddressHandler,
            ICommandHandler<DeleteAddressCommand, bool> deleteAddressHandler)
        {
            _getMyAddressesHandler = getMyAddressesHandler;
            _getAddressDetailHandler = getAddressDetailHandler;
            _createAddressHandler = createAddressHandler;
            _updateAddressHandler = updateAddressHandler;
            _deleteAddressHandler = deleteAddressHandler;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<AddressDto>>>> GetMyAddresses()
        {
            var result = await _getMyAddressesHandler.Handle(new GetMyAddressesQuery());
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<AddressDto>>> GetAddressById(int id)
        {
            var result = await _getAddressDetailHandler.Handle(new GetAddressDetailQuery(id));
            return HandleResult(result);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> CreateAddress([FromBody] CreateAddressCommand command)
        {
            var result = await _createAddressHandler.Handle(command);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse<int>.Failure(result.Error ?? "Tạo địa chỉ thất bại."));
            }

            return CreatedAtAction(
                nameof(GetAddressById),
                new { id = result.Value },
                ApiResponse<int>.Success(result.Value, "Tạo địa chỉ thành công."));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateAddress(int id, [FromBody] UpdateAddressCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest(ApiResponse<bool>.Failure("ID trong URL và body không khớp"));
            }

            var result = await _updateAddressHandler.Handle(command);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteAddress(int id)
        {
            var result = await _deleteAddressHandler.Handle(new DeleteAddressCommand { Id = id });
            return HandleResult(result);
        }
    }
}
