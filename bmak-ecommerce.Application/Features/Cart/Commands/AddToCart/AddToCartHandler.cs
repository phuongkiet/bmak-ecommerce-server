using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Cart.Models;
using bmak_ecommerce.Domain.Interfaces;

namespace bmak_ecommerce.Application.Features.Cart.Commands.AddToCart
{
    [AutoRegister]

    public class AddToCartHandler : ICommandHandler<AddToCartCommand, ShoppingCart>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public AddToCartHandler(ICartRepository cartRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<Result<ShoppingCart>> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            // 1. Lấy thông tin sản phẩm chuẩn từ DB
            // (Phải tin tưởng DB, không tin tưởng dữ liệu FE gửi lên ngoại trừ ID và Quantity)
            var product = await _productRepository.GetByIdAsync(request.ProductId);

            if (product == null)
            {
                return Result<ShoppingCart>.Failure("Sản phẩm không tồn tại.");
            }

            if (!product.IsActive)
            {
                return Result<ShoppingCart>.Failure($"Sản phẩm '{product.Name}' hiện đang ngừng kinh doanh.");
            }

            // 2. Lấy giỏ hàng hiện tại từ Redis (Nếu chưa có thì tạo mới)
            var cart = await _cartRepository.GetCartAsync(request.CartId)
                       ?? new ShoppingCart(request.CartId);

            // 3. Xử lý logic thêm/sửa item
            var existingItem = cart.Items.FirstOrDefault(x => x.ProductId == request.ProductId);

            // Xác định giá bán (Ưu tiên giá Sale)
            var currentPrice = product.SalePrice > 0 ? product.SalePrice : product.BasePrice;

            if (existingItem != null)
            {
                // CASE A: Sản phẩm đã có trong giỏ -> Cộng dồn
                existingItem.Quantity += request.Quantity;

                // QUAN TRỌNG: Cập nhật lại giá và thông tin mới nhất từ DB
                // Để tránh trường hợp Admin vừa đổi giá hoặc đổi ảnh
                existingItem.Price = currentPrice;
                existingItem.OriginalPrice = product.BasePrice;
                existingItem.PictureUrl = product.Thumbnail;
                existingItem.ConversionFactor = product.ConversionFactor; // Cập nhật lại hệ số nếu có thay đổi
            }
            else
            {
                // CASE B: Chưa có -> Tạo mới Item
                var item = new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductSku = product.SKU,
                    PictureUrl = product.Thumbnail,

                    // Logic giá
                    Price = currentPrice,
                    OriginalPrice = product.BasePrice,

                    // Logic số lượng
                    Quantity = request.Quantity,

                    // Logic Gạch men (Lưu để FE hiển thị và tính m2)
                    SalesUnit = product.SalesUnit,
                    PriceUnit = !string.IsNullOrEmpty(product.PriceUnit) ? product.PriceUnit : "m²",
                    ConversionFactor = product.ConversionFactor > 0 ? product.ConversionFactor : 1
                };

                cart.Items.Add(item);
            }

            // 4. Lưu ngược lại vào Redis
            var updatedCart = await _cartRepository.UpdateCartAsync(cart);

            if (updatedCart == null)
            {
                return Result<ShoppingCart>.Failure("Lỗi khi lưu giỏ hàng.");
            }

            return Result<ShoppingCart>.Success(updatedCart);
        }
    }
}
