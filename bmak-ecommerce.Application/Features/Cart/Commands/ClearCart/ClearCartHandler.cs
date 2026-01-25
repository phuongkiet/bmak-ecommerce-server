using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Features.Cart.Commands.DeleteCartItem;
using bmak_ecommerce.Application.Features.Cart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Cart.Commands.ClearCart
{
    public class ClearCartHandler : ICommandHandler<ClearCartCommand, ShoppingCart>
    {
        private readonly ICartRepository _cartRepository;

        public ClearCartHandler(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<ShoppingCart> Handle(ClearCartCommand request, CancellationToken cancellationToken)
        {
            await _cartRepository.DeleteCartAsync(request.CartId);

            // 2. Trả về một giỏ hàng rỗng mới tinh để Frontend cập nhật UI
            // Nếu không trả về gì (void), Frontend sẽ không biết state mới là gì
            return new ShoppingCart(request.CartId);
        }
    }
}
