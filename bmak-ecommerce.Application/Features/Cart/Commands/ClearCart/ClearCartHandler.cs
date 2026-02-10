using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Cart.Models;

namespace bmak_ecommerce.Application.Features.Cart.Commands.ClearCart
{
    [AutoRegister]

    public class ClearCartHandler : ICommandHandler<ClearCartCommand, ShoppingCart>
    {
        private readonly ICartRepository _cartRepository;

        public ClearCartHandler(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<Result<ShoppingCart>> Handle(ClearCartCommand request, CancellationToken cancellationToken)
        {
            // Gọi Redis xóa key
            await _cartRepository.DeleteCartAsync(request.CartId);

            // Trả về giỏ hàng rỗng
            var emptyCart = new ShoppingCart
            {
                Id = request.CartId,
                Items = new List<CartItem>()
            };

            return Result<ShoppingCart>.Success(emptyCart);
        }
    }
}
