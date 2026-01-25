using bmak_ecommerce.Application.Common.Interfaces;
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
    public class CartController : ControllerBase
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

        // GET: api/cart?id=guest-123
        [HttpGet]
        public async Task<ActionResult<ShoppingCart>> GetCart(string id)
        {
            var result = await _getCartHandler.Handle(new GetCartQuery(id));
            return Ok(result);
        }

        // POST: api/cart
        [HttpPost]
        public async Task<ActionResult<ShoppingCart>> AddToCart(AddToCartCommand command)
        {
            var result = await _addToCartHandler.Handle(command);
            return Ok(result);
        }

        [HttpPut]
        public async Task<ActionResult<ShoppingCart>> UpdateCartItem(UpdateCartItemCommand command)
        {
            try
            {
                var result = await _updateCartItemHandler.Handle(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("item")]
        public async Task<ActionResult<ShoppingCart>> DeleteCartItem([FromQuery] DeleteCartItemCommand command)
        {
            try
            {
                var result = await _deleteCartItemHandler.Handle(command, CancellationToken.None);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<ActionResult<ShoppingCart>> ClearCart([FromQuery] ClearCartCommand command)
        {
            try
            {
                var result = await _clearCartHandler.Handle(command, CancellationToken.None);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
