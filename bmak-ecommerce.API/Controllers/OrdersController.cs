using Microsoft.AspNetCore.Mvc;
using bmak_ecommerce.Application.Features.Orders.Commands.CreateOrder;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Features.Products.DTOs.Sale;
using bmak_ecommerce.Application.Features.Products.Queries.Orders.GetAllOrders;
using bmak_ecommerce.Domain.Models;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.API.Extensions;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IQueryHandler<GetOrdersQuery, Result<PagedList<OrderSummaryDto>>> _getOrdersHandler;
    public OrdersController(IQueryHandler<GetOrdersQuery, Result<PagedList<OrderSummaryDto>>> getOrdersHandler)
    {
        _getOrdersHandler = getOrdersHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders([FromQuery] OrderSpecParams specParams)
    {
        var query = new GetOrdersQuery(specParams);
        var result = await _getOrdersHandler.Handle(query);

        if (result.IsSuccess)
        {
            // Bây giờ result đã hiểu .Value và .IsSuccess
            Response.AddPaginationHeader(
                result.Value.PageIndex,
                result.Value.PageSize,
                result.Value.TotalCount,
                result.Value.TotalPages);

            return Ok(result.Value);
        }

        return BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(
        [FromServices] ICreateOrderCommandHandler handler,
        [FromBody] CreateOrderCommand command)
    {
        var result = await handler.HandleAsync(
            command,
            HttpContext.RequestAborted);

        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                message = result.Error
            });
        }

        return Ok(new
        {
            message = "Đặt hàng thành công",
            orderId = result.Value
        });
    }
}