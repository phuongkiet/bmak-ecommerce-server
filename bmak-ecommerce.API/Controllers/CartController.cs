using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Cart.Commands.AddToCart;
using bmak_ecommerce.Application.Features.Cart.Commands.ClearCart;
using bmak_ecommerce.Application.Features.Cart.Commands.DeleteCartItem;
using bmak_ecommerce.Application.Features.Cart.Commands.UpdateCartItem;
using bmak_ecommerce.Application.Features.Cart.Models;
using bmak_ecommerce.Application.Features.Cart.Queries.GetCart;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace bmak_ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : BaseApiController
    {
        private readonly IQueryHandler<GetCartQuery, ShoppingCart> _getCartHandler;
        private readonly ICommandHandler<AddToCartCommand, ShoppingCart> _addToCartHandler;
        private readonly ICommandHandler<UpdateCartItemCommand, ShoppingCart> _updateCartItemHandler;
        private readonly ICommandHandler<DeleteCartItemCommand, ShoppingCart> _deleteCartItemHandler;
        private readonly ICommandHandler<ClearCartCommand, ShoppingCart> _clearCartHandler;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<CartController> _logger;

        public CartController(
            IQueryHandler<GetCartQuery, ShoppingCart> getCartHandler,
            ICommandHandler<AddToCartCommand, ShoppingCart> addToCartHandler,
            ICommandHandler<UpdateCartItemCommand, ShoppingCart> updateCartItemHandler,
            ICommandHandler<DeleteCartItemCommand, ShoppingCart> deleteCartItemHandler,
            ICommandHandler<ClearCartCommand, ShoppingCart> clearCartHandler,
            ICurrentUserService currentUserService,
            ILogger<CartController> logger)
        {
            _getCartHandler = getCartHandler;
            _addToCartHandler = addToCartHandler;
            _updateCartItemHandler = updateCartItemHandler;
            _deleteCartItemHandler = deleteCartItemHandler;
            _clearCartHandler = clearCartHandler;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        private string GetEffectiveCartId(string clientCartId)
        {
            var isAuth = User.Identity?.IsAuthenticated == true;
            if (isAuth)
            {
                if (string.IsNullOrWhiteSpace(_currentUserService.UserId.ToString()))
                {
                    _logger.LogWarning("Authenticated user but UserId is empty. clientCartId={ClientCartId}", clientCartId);
                    throw new UnauthorizedAccessException("UserId is missing.");
                }

                var effective = $"cart:user:{_currentUserService.UserId}";
                _logger.LogInformation("EffectiveCartId resolved for user. clientCartId={ClientCartId}, effectiveCartId={EffectiveCartId}", clientCartId, effective);
                return effective;
            }

            _logger.LogInformation("EffectiveCartId resolved for guest. clientCartId={ClientCartId}", clientCartId);
            return clientCartId;
        }

        // GET: api/cart?id=cart-123
        [HttpGet]
        public async Task<ActionResult<ApiResponse<ShoppingCart>>> GetCart([FromQuery] string id)
        {
            var query = new GetCartQuery(GetEffectiveCartId(id));
            var result = await _getCartHandler.Handle(query);
            return HandleResult(result);
        }

        // POST: api/cart (Add Item)
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ShoppingCart>>> AddToCart([FromBody] AddToCartCommand command)
        {
            command.CartId = GetEffectiveCartId(command.CartId);
            var result = await _addToCartHandler.Handle(command);
            return HandleResult(result);
        }

        // PUT: api/cart (Update Quantity)
        [HttpPut]
        public async Task<ActionResult<ApiResponse<ShoppingCart>>> UpdateItem([FromBody] UpdateCartItemCommand command)
        {
            command.CartId = GetEffectiveCartId(command.CartId);
            var result = await _updateCartItemHandler.Handle(command);
            return HandleResult(result);
        }

        // PUT: api/cart/item?cartId=xxx&productId=1 (Remove Item)
        [HttpPut("item")]
        public async Task<ActionResult<ApiResponse<ShoppingCart>>> RemoveItem([FromQuery] string cartId, [FromQuery] int productId)
        {
            var command = new DeleteCartItemCommand { CartId = GetEffectiveCartId(cartId), ProductId = productId };
            var result = await _deleteCartItemHandler.Handle(command);
            return HandleResult(result);
        }

        [HttpDelete]
        public async Task<ActionResult<ApiResponse<ShoppingCart>>> ClearCart([FromQuery] ClearCartCommand command)
        {
            command.CartId = GetEffectiveCartId(command.CartId);
            try
            {
                var result = await _clearCartHandler.Handle(command, CancellationToken.None);
                if (result.IsSuccess)
                    return Ok(ApiResponse<ShoppingCart>.Success(result.Value, "Cart cleared successfully"));
                return BadRequest(ApiResponse<ShoppingCart>.Failure(result.Error ?? "Failed to clear cart"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<ShoppingCart>.Failure(ex.Message));
            }
        }
    }
}
