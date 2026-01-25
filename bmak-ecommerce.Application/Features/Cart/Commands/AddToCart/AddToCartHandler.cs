using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Features.Cart.Models;
using bmak_ecommerce.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Cart.Commands.AddToCart
{
    public class AddToCartHandler : ICommandHandler<AddToCartCommand, ShoppingCart>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository; // Cần repo này để lấy giá chuẩn

        public AddToCartHandler(ICartRepository cartRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<ShoppingCart> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            // 1. Lấy thông tin sản phẩm chuẩn từ DB (Không tin tưởng FE gửi giá lên)
            var product = await _productRepository.GetByIdAsync(request.ProductId);

            if (product == null)
                throw new Exception("Sản phẩm không tồn tại"); // Hoặc return Result.Failure

            // 2. Lấy giỏ hàng hiện tại từ Redis (Nếu chưa có thì tạo mới)
            var cart = await _cartRepository.GetCartAsync(request.CartId)
                       ?? new ShoppingCart(request.CartId);

            // 3. Kiểm tra sản phẩm đã có trong giỏ chưa
            var existingItem = cart.Items.FirstOrDefault(x => x.ProductId == request.ProductId);

            if (existingItem != null)
            {
                // Nếu có rồi -> Cộng dồn số lượng
                existingItem.Quantity += request.Quantity;

                // Cập nhật lại giá mới nhất (phòng trường hợp Admin vừa đổi giá)
                existingItem.Price = product.SalePrice > 0 ? product.SalePrice : product.BasePrice;
            }
            else
            {
                // Nếu chưa có -> Tạo mới Item
                var item = new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductSlug = product.Slug,
                    PictureUrl = product.Thumbnail,

                    // Logic giá
                    Price = product.SalePrice > 0 ? product.SalePrice : product.BasePrice,
                    OriginalPrice = product.BasePrice,

                    Quantity = request.Quantity,

                    // Logic Gạch men (Quan trọng)
                    SalesUnit = product.SalesUnit,
                    PriceUnit = product.PriceUnit,
                    ConversionFactor = product.ConversionFactor
                };
                cart.Items.Add(item);
            }

            // 4. Lưu ngược lại vào Redis
            var updatedCart = await _cartRepository.UpdateCartAsync(cart);
            return updatedCart!;
        }
    }
}
