using bmak_ecommerce.API.Extensions;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Orders.Commands.UpdateOrderStatus;
using bmak_ecommerce.Application.Features.Orders.DTOs;
using bmak_ecommerce.Application.Features.Orders.Queries.GetAllOrders;
using bmak_ecommerce.Application.Features.Orders.Queries.GetOrderById;
using bmak_ecommerce.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bmak_ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/admin/orders")]
    [Authorize(Roles = "Admin")]
    public class AdminOrdersController : BaseApiController
    {
        private readonly IQueryHandler<GetOrdersQuery, PagedList<OrderSummaryDto>> _getOrdersHandler;
        private readonly IQueryHandler<GetOrderByIdQuery, OrderDto?> _getOrderByIdHandler;
        private readonly ICommandHandler<UpdateOrderStatusCommand, bool> _updateOrderStatusHandler;

        public AdminOrdersController(
            IQueryHandler<GetOrdersQuery, PagedList<OrderSummaryDto>> getOrdersHandler,
            IQueryHandler<GetOrderByIdQuery, OrderDto?> getOrderByIdHandler,
            ICommandHandler<UpdateOrderStatusCommand, bool> updateOrderStatusHandler)
        {
            _getOrdersHandler = getOrdersHandler;
            _getOrderByIdHandler = getOrderByIdHandler;
            _updateOrderStatusHandler = updateOrderStatusHandler;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedList<OrderSummaryDto>>>> GetOrders([FromQuery] OrderSpecParams specParams)
        {
            var result = await _getOrdersHandler.Handle(new GetOrdersQuery(specParams));

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

        [HttpGet("{orderCode}")]
        public async Task<ActionResult<ApiResponse<OrderDto>>> GetOrderByCode(string orderCode)
        {
            var result = await _getOrderByIdHandler.Handle(new GetOrderByIdQuery(orderCode), CancellationToken.None);
            return HandleResult(result);
        }

        [HttpPatch("{orderCode}/status")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateOrderStatus(
            string orderCode,
            [FromBody] UpdateOrderStatusCommand command)
        {
            command.OrderCode = orderCode;
            var result = await _updateOrderStatusHandler.Handle(command);
            return HandleResult(result);
        }
    }
}
