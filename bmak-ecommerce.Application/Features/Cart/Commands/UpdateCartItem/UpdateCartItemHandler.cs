using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Cart.Models;

namespace bmak_ecommerce.Application.Features.Cart.Commands.UpdateCartItem
{
    [AutoRegister]

    public class UpdateCartItemHandler : ICommandHandler<UpdateCartItemCommand, ShoppingCart>
    {
        private readonly ICartRepository _cartRepository;

        public UpdateCartItemHandler(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<Result<ShoppingCart>> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
        {
            // 1. Lấy giỏ hàng
            var cart = await _cartRepository.GetCartAsync(request.CartId);

            if (cart == null)
            {
                return Result<ShoppingCart>.Failure("Giỏ hàng không tồn tại.");
            }

            // 2. Tìm sản phẩm
            var item = cart.Items.FirstOrDefault(x => x.ProductId == request.ProductId);

            if (item == null)
            {
                return Result<ShoppingCart>.Failure("Sản phẩm không có trong giỏ.");
            }

            // 3. Cập nhật số lượng
            // Logic: Nếu số lượng > 0 thì update, nếu <= 0 thì xóa luôn
            if (request.Quantity > 0)
            {
                item.Quantity = request.Quantity;
                // Lưu ý: Vì CartItem có thuộc tính computed QuantitySquareMeter 
                // nên nó sẽ tự động tính lại m2 khi Quantity thay đổi.
            }
            else
            {
                cart.Items.Remove(item);
            }

            // 4. Lưu lại Redis
            var updatedCart = await _cartRepository.UpdateCartAsync(cart);

            return Result<ShoppingCart>.Success(updatedCart!);
        }
    }
}
