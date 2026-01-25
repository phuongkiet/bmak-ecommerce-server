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
    private readonly ICommandHandler<CreateOrderCommand, Result<int>> _createOrderHandler;
    public OrdersController(IQueryHandler<GetOrdersQuery, Result<PagedList<OrderSummaryDto>>> getOrdersHandler,
        ICommandHandler<CreateOrderCommand, Result<int>> createOrderHandler)
    {
        _getOrdersHandler = getOrdersHandler;
        _createOrderHandler = createOrderHandler;
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
    public async Task<ActionResult<Result<int>>> CreateOrder([FromBody] CreateOrderCommand command)
    {
        // Gọi hàm Handle trực tiếp
        var result = await _createOrderHandler.Handle(command, CancellationToken.None);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}