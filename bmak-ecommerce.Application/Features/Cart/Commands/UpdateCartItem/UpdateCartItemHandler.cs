using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Features.Cart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Cart.Commands.UpdateCartItem
{
    public class UpdateCartItemHandler : ICommandHandler<UpdateCartItemCommand, ShoppingCart>
    {
        private readonly ICartRepository _cartRepository;

        public UpdateCartItemHandler(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<ShoppingCart> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
        {
            // 1. Lấy giỏ hàng từ Redis
            var cart = await _cartRepository.GetCartAsync(request.CartId);

            if (cart == null)
            {
                // Tùy nghiệp vụ: Có thể throw lỗi hoặc trả về giỏ rỗng
                throw new Exception("Giỏ hàng không tồn tại");
            }

            // 2. Tìm sản phẩm cần update
            var item = cart.Items.FirstOrDefault(x => x.ProductId == request.ProductId);

            if (item == null)
            {
                throw new Exception("Sản phẩm không có trong giỏ hàng");
            }

            // 3. Cập nhật số lượng
            item.Quantity = request.Quantity;

            // 4. QUAN TRỌNG: Tính lại số m2 (QuantitySquareMeter)
            // Vì Property QuantitySquareMeter trong CartItem là "computed property" (chỉ có get) 
            // nên ta không cần gán thủ công nếu bạn đã dùng cú pháp `=>` trong model CartItem.
            // Tuy nhiên, nếu bạn lưu cứng giá trị này trong Redis (để search/filter), thì cần tính lại.

            // (Xem lại CartItem Model ở bước trước: public float QuantitySquareMeter => Quantity * ...)
            // -> Khi Quantity đổi, QuantitySquareMeter tự động đổi khi serialize/deserialize.

            // 5. Lưu ngược lại vào Redis
            var updatedCart = await _cartRepository.UpdateCartAsync(cart);
            return updatedCart!;
        }
    }
}
