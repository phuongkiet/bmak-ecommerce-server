using bmak_ecommerce.API.Extensions;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Models;
using bmak_ecommerce.Application.Features.Orders.Commands.CreateOrder;
using Microsoft.AspNetCore.Mvc;
using bmak_ecommerce.Application.Features.Orders.Queries.GetAllOrdersOfUser;
using bmak_ecommerce.Application.Features.Orders.DTOs;
using bmak_ecommerce.Application.Features.Orders.Queries.GetOrderById;
using bmak_ecommerce.Application.Features.Orders.Queries.CalculateShippingFee;
using Microsoft.AspNetCore.Authorization;

namespace bmak_ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : BaseApiController
    {
        private readonly ICommandHandler<CreateOrderCommand, int> _createOrderHandler;
        private readonly IQueryHandler<GetAllOrdersOfUserQuery, PagedList<OrderSummaryDto>> _getOdersOfUserHandler;
        private readonly IQueryHandler<GetOrderByIdQuery, OrderDto?> _getOrderByIdHandler;
        private readonly IQueryHandler<CalculateShippingFeeQuery, ShippingFeeResponseDto> _calculateShippingFeeHandler;
        private readonly ICurrentUserService _currentUserService;

        public OrdersController(
            ICommandHandler<CreateOrderCommand, int> createOrderHandler,
            IQueryHandler<GetAllOrdersOfUserQuery, PagedList<OrderSummaryDto>> getOdersOfUserHandler,
            IQueryHandler<GetOrderByIdQuery, OrderDto?> getOrderByIdHandler,
            IQueryHandler<CalculateShippingFeeQuery, ShippingFeeResponseDto> calculateShippingFeeHandler,
            ICurrentUserService currentUserService)
        {
            _createOrderHandler = createOrderHandler;
            _getOdersOfUserHandler = getOdersOfUserHandler;
            _getOrderByIdHandler = getOrderByIdHandler;
            _calculateShippingFeeHandler = calculateShippingFeeHandler;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PagedList<OrderSummaryDto>>>> GetOrders([FromQuery] OrderSpecParams specParams)
        {
            if (_currentUserService.UserId <= 0)
            {
                return Unauthorized(ApiResponse<PagedList<OrderSummaryDto>>.Failure("Unauthorized"));
            }

            specParams.UserId = _currentUserService.UserId;
            var result = await _getOdersOfUserHandler.Handle(new GetAllOrdersOfUserQuery(specParams));

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
        [Authorize]
        public async Task<ActionResult<ApiResponse<OrderDto>>> GetById(string orderCode)
        {
            if (_currentUserService.UserId <= 0)
            {
                return Unauthorized(ApiResponse<OrderDto>.Failure("Unauthorized"));
            }

            var result = await _getOrderByIdHandler.Handle(new GetOrderByIdQuery(orderCode), CancellationToken.None);
            if (result.IsSuccess && result.Value != null && result.Value.UserId != _currentUserService.UserId)
            {
                return Forbid();
            }

            return HandleResult(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResponse<int>>> CreateOrder([FromBody] CreateOrderCommand command)
        {
            var result = await _createOrderHandler.Handle(command);
            return HandleResult(result);
        }

        [HttpPost("shipping-fee")]
        public async Task<ActionResult<ApiResponse<ShippingFeeResponseDto>>> CalculateShippingFee([FromBody] CalculateShippingFeeQuery query)
        {
            var result = await _calculateShippingFeeHandler.Handle(query);
            return HandleResult(result);
        }
    }
}