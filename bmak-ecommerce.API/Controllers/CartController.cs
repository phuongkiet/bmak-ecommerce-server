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

        public CartController(
            IQueryHandler<GetCartQuery, ShoppingCart> getCartHandler,
            ICommandHandler<AddToCartCommand, ShoppingCart> addToCartHandler,
            ICommandHandler<UpdateCartItemCommand, ShoppingCart> updateCartItemHandler,
            ICommandHandler<DeleteCartItemCommand,ShoppingCart> deleteCartItemHandler,
            ICommandHandler<ClearCartCommand, ShoppingCart> clearCartHandler)
        {
            _getCartHandler = getCartHandler;
            _addToCartHandler = addToCartHandler;
            _updateCartItemHandler = updateCartItemHandler;
            _deleteCartItemHandler = deleteCartItemHandler;
            _clearCartHandler = clearCartHandler;
        }

        // GET: api/cart?id=cart-123
        [HttpGet]
        public async Task<ActionResult<ApiResponse<ShoppingCart>>> GetCart([FromQuery] string id)
        {
            var query = new GetCartQuery(id);
            var result = await _getCartHandler.Handle(query);
            return HandleResult(result);
        }

        // POST: api/cart (Add Item)
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ShoppingCart>>> AddToCart([FromBody] AddToCartCommand command)
        {
            var result = await _addToCartHandler.Handle(command);
            return HandleResult(result);
        }

        // PUT: api/cart (Update Quantity)
        [HttpPut]
        public async Task<ActionResult<ApiResponse<ShoppingCart>>> UpdateItem([FromBody] UpdateCartItemCommand command)
        {
            var result = await _updateCartItemHandler.Handle(command);
            return HandleResult(result);
        }

        // DELETE: api/cart/cart-123/1 (Remove Item)
        [HttpPut("item")]
        public async Task<ActionResult<ApiResponse<ShoppingCart>>> RemoveItem(string cartId, int productId)
        {
            var command = new DeleteCartItemCommand { CartId = cartId, ProductId = productId };
            var result = await _deleteCartItemHandler.Handle(command);
            return HandleResult(result);
        }

        [HttpDelete]
        public async Task<ActionResult<ApiResponse<ShoppingCart>>> ClearCart([FromQuery] ClearCartCommand command)
        {
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
