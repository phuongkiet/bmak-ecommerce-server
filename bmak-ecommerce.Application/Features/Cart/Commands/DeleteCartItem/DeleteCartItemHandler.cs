using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Features.Cart.Models;

namespace bmak_ecommerce.Application.Features.Cart.Commands.DeleteCartItem
{
    public class DeleteCartItemHandler : ICommandHandler<DeleteCartItemCommand, ShoppingCart>
    {
        private readonly ICartRepository _cartRepository;

        public DeleteCartItemHandler(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<ShoppingCart> Handle(DeleteCartItemCommand request, CancellationToken cancellationToken)
        {
            // 1. Lấy giỏ hàng
            var cart = await _cartRepository.GetCartAsync(request.CartId);

            if (cart == null) throw new Exception("Giỏ hàng không tồn tại");

            // 2. Tìm sản phẩm
            var item = cart.Items.FirstOrDefault(x => x.ProductId == request.ProductId);

            if (item == null) throw new Exception("Sản phẩm không có trong giỏ hàng");

            // 3. Xóa item khỏi list (Logic quan trọng)
            cart.Items.Remove(item);

            // 4. Lưu lại giỏ hàng đã update vào Redis
            // (Hàm UpdateCartAsync sẽ ghi đè giỏ hàng cũ bằng giỏ hàng mới đã mất 1 item)
            var updatedCart = await _cartRepository.UpdateCartAsync(cart);

            return updatedCart!;
        }
    }
}