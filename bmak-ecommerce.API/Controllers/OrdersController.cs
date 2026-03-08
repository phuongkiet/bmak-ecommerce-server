using bmak_ecommerce.API.Extensions;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Models;
using bmak_ecommerce.Application.Features.Orders.Commands.CreateOrder;
using Microsoft.AspNetCore.Mvc;
using bmak_ecommerce.Application.Features.Orders.Queries.GetAllOrders;
using bmak_ecommerce.Application.Features.Orders.Queries.GetAllOrdersOfUser;
using bmak_ecommerce.Application.Features.Orders.DTOs;
using bmak_ecommerce.Application.Features.Orders.Queries.GetOrderById;
using bmak_ecommerce.Application.Features.Orders.Queries.CalculateShippingFee;

namespace bmak_ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : BaseApiController
    {
        // 1. Sửa Generic Type: Bỏ Result<> bao bọc bên ngoài
        private readonly IQueryHandler<GetOrdersQuery, PagedList<OrderSummaryDto>> _getOrdersHandler;
        private readonly ICommandHandler<CreateOrderCommand, int> _createOrderHandler;
        private readonly IQueryHandler<GetAllOrdersOfUserQuery, PagedList<OrderSummaryDto>> _getOdersOfUserHandler;
        private readonly IQueryHandler<GetOrderByIdQuery, OrderDto?> _getOrderByIdHandler;
        private readonly IQueryHandler<CalculateShippingFeeQuery, ShippingFeeResponseDto> _calculateShippingFeeHandler;

        public OrdersController(
            IQueryHandler<GetOrdersQuery, PagedList<OrderSummaryDto>> getOrdersHandler,
            ICommandHandler<CreateOrderCommand, int> createOrderHandler,
            IQueryHandler<GetAllOrdersOfUserQuery, PagedList<OrderSummaryDto>> getOdersOfUserHandler,
            IQueryHandler<GetOrderByIdQuery, OrderDto?> getOrderByIdHandler,
            IQueryHandler<CalculateShippingFeeQuery, ShippingFeeResponseDto> calculateShippingFeeHandler)
        {
            _getOrdersHandler = getOrdersHandler;
            _createOrderHandler = createOrderHandler;
            _getOdersOfUserHandler = getOdersOfUserHandler;
            _getOrderByIdHandler = getOrderByIdHandler;
            _calculateShippingFeeHandler = calculateShippingFeeHandler;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedList<OrderSummaryDto>>>> GetOrders([FromQuery] OrderSpecParams specParams)
        {
            var query = new GetOrdersQuery(specParams);

            // Result trả về là Result<PagedList<OrderSummaryDto>>
            var result = await _getOrdersHandler.Handle(query);

            // Xử lý Pagination Header nếu thành công
            if (result.IsSuccess && result.Value != null)
            {
                Response.AddPaginationHeader(
                    result.Value.PageIndex,
                    result.Value.PageSize,
                    result.Value.TotalCount,
                    result.Value.TotalPages
                );
            }

            // Dùng HandleResult của BaseApiController
            return HandleResult(result);
        }

        [HttpGet("user")]
        public async Task<ActionResult<ApiResponse<PagedList<OrderSummaryDto>>>> GetOrdersOfUser([FromQuery] OrderSpecParams specParams)
        {
            var query = new GetAllOrdersOfUserQuery(specParams);

            // Result trả về là Result<PagedList<OrderSummaryDto>>
            var result = await _getOdersOfUserHandler.Handle(query);

            // Xử lý Pagination Header nếu thành công
            if (result.IsSuccess && result.Value != null)
            {
                Response.AddPaginationHeader(
                    result.Value.PageIndex,
                    result.Value.PageSize,
                    result.Value.TotalCount,
                    result.Value.TotalPages
                );
            }

            // Dùng HandleResult của BaseApiController
            return HandleResult(result);
        }

        [HttpGet("{orderCode}")]
        public async Task<ActionResult<ApiResponse<OrderDto>>> GetById(string orderCode)
        {
            var query = new GetOrderByIdQuery(orderCode);

            // Gọi Handler -> nhận Result<ProductDto?>
            var result = await _getOrderByIdHandler.Handle(query, CancellationToken.None);

            // Trả về HTTP 200 (Success) hoặc 404 (Failure)
            return HandleResult(result);
        }

        // POST: api/orders
        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> CreateOrder([FromBody] CreateOrderCommand command)
        {
            // Frontend gửi: { "cartId": "...", "shippingAddress": ... }
            // Result trả về là Result<int> (ID đơn hàng)
            var result = await _createOrderHandler.Handle(command);

            // Nếu thành công, trả về 200 OK kèm ID đơn hàng
            // Nếu thất bại (lỗi validate, hết hàng...), trả về 400 Bad Request kèm message
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