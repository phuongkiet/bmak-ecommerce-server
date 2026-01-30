using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Cart.Models;

namespace bmak_ecommerce.Application.Features.Cart.Queries.GetCart
{
    public class GetCartHandler : IQueryHandler<GetCartQuery, ShoppingCart>
    {
        private readonly ICartRepository _cartRepository;

        public GetCartHandler(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<Result<ShoppingCart>> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetCartAsync(request.CartId);

            // Nếu null (chưa có trong Redis) -> Trả về giỏ mới tinh
            return Result<ShoppingCart>.Success(cart ?? new ShoppingCart(request.CartId));
        }
    }
}
