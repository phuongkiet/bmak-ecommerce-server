using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Cart.Models;

namespace bmak_ecommerce.Application.Features.Cart.Commands.DeleteCartItem
{
    [AutoRegister]

    public class DeleteCartItemHandler :ICommandHandler<DeleteCartItemCommand, ShoppingCart>
    {
        private readonly ICartRepository _cartRepository;

        public DeleteCartItemHandler(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<Result<ShoppingCart>> Handle(DeleteCartItemCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetCartAsync(request.CartId);

            if (cart == null) return Result<ShoppingCart>.Failure("Giỏ hàng không tồn tại.");

            // Tìm và xóa
            var item = cart.Items.FirstOrDefault(x => x.ProductId == request.ProductId);
            if (item != null)
            {
                cart.Items.Remove(item);

                // Lưu lại
                var updatedCart = await _cartRepository.UpdateCartAsync(cart);
                return Result<ShoppingCart>.Success(updatedCart!);
            }

            // Nếu không tìm thấy item, coi như đã xóa thành công (Idempotency)
            return Result<ShoppingCart>.Success(cart);
        }
    }
}