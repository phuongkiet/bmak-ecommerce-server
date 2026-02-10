using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Entities.Sales;
using bmak_ecommerce.Domain.Enums;
using bmak_ecommerce.Domain.Interfaces;

namespace bmak_ecommerce.Application.Features.Orders.Commands.CreateOrder
{
    [AutoRegister]

    public class CreateOrderHandler : ICommandHandler<CreateOrderCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartRepository _cartRepository; // Service lấy giỏ hàng (Redis/DB)
        private readonly ICurrentUserService _currentUserService; // Lấy UserId đăng nhập

        public CreateOrderHandler(
            IUnitOfWork unitOfWork,
            ICartRepository cartRepository,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _cartRepository = cartRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<int>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // 1. LẤY GIỎ HÀNG
            var cart = await _cartRepository.GetCartAsync(request.CartId);

            if (cart == null || !cart.Items.Any())
            {
                return Result<int>.Failure("Giỏ hàng trống hoặc không tồn tại.");
            }

            // 2. KIỂM TRA TỒN KHO & GIÁ (Validate Stock)
            // Lấy danh sách ProductId trong giỏ
            var productIds = cart.Items.Select(x => x.ProductId).ToList();

            // Load Products từ DB để check tồn kho thực tế
            // (Cần viết hàm GetProductsByIdsAsync trong Repo bao gồm cả Stocks)
            var dbProducts = await _unitOfWork.Products.GetByIdsAsync(productIds);

            foreach (var cartItem in cart.Items)
            {
                var dbProduct = dbProducts.FirstOrDefault(p => p.Id == cartItem.ProductId);

                if (dbProduct == null)
                    return Result<int>.Failure($"Sản phẩm ID {cartItem.ProductId} không còn tồn tại.");

                if (!dbProduct.IsActive)
                    return Result<int>.Failure($"Sản phẩm '{dbProduct.Name}' đang ngừng kinh doanh.");

                // Check số lượng tồn (Tổng các kho)
                var currentStock = dbProduct.Stocks.Sum(s => s.QuantityOnHand);

                // Nếu có quản lý kho và không cho bán âm
                if (dbProduct.ManageStock && !dbProduct.AllowBackorder && currentStock < cartItem.Quantity)
                {
                    return Result<int>.Failure($"Sản phẩm '{dbProduct.Name}' không đủ hàng (Còn: {currentStock}).");
                }
            }

            // 3. XỬ LÝ LOGIC ĐỊA CHỈ (Billing vs Shipping)
            string finalReceiverName, finalReceiverPhone, finalShippingAddress;
            string finalBillingAddress = request.BillingAddress.ToString();

            if (request.ShipToDifferentAddress)
            {
                // Validate server side cho chắc
                if (request.ShippingAddress == null)
                    return Result<int>.Failure("Chưa nhập địa chỉ giao hàng.");

                finalReceiverName = request.ReceiverName;
                finalReceiverPhone = request.ReceiverPhone;
                finalShippingAddress = request.ShippingAddress.ToString();
            }
            else
            {
                // Copy từ Billing sang
                finalReceiverName = request.BuyerName;
                finalReceiverPhone = request.BuyerPhone;
                finalShippingAddress = finalBillingAddress;
            }

            // 4. TẠO ORDER ENTITY
            var order = new Order
            {
                OrderCode = GenerateOrderCode(), // Hàm tự viết: ORDER-20231025-1234
                Status = OrderStatus.Pending,    // Mới tạo -> Chờ xử lý
                PaymentMethod = request.PaymentMethod,
                Note = request.Note,
                OrderDate = DateTime.UtcNow,
                UserId = _currentUserService.UserId != 0 ? _currentUserService.UserId : 1, // Nếu guest thì null

                // Snapshot thông tin người mua
                BuyerName = request.BuyerName,
                BuyerPhone = request.BuyerPhone,
                BuyerEmail = request.BuyerEmail,
                BillingAddress = finalBillingAddress,

                // Snapshot thông tin nhận hàng
                ReceiverName = finalReceiverName,
                ReceiverPhone = finalReceiverPhone,
                ShippingAddress = finalShippingAddress,

                TotalAmount = 0 // Sẽ cộng dồn ở dưới
            };

            // 5. TẠO ORDER ITEMS & TRỪ KHO
            foreach (var cartItem in cart.Items)
            {
                var dbProduct = dbProducts.FirstOrDefault(p => p.Id == cartItem.ProductId);

                // 1. Lấy hệ số quy đổi (Ví dụ: 1 viên = 0.36m2)
                // Nếu ConversionFactor = 0 hoặc null thì mặc định là 1 để tránh lỗi chia/nhân 0
                float factor = dbProduct.ConversionFactor > 0 ? dbProduct.ConversionFactor : 1;

                // 2. Tính ra số m2 cần trừ kho
                // Ví dụ: Khách mua 10 viên => 10 * 0.36 = 3.6 m2
                float quantityM2 = (float)cartItem.Quantity * factor;

                // 3. TẠO ORDER ITEM
                var orderItem = new OrderItem
                {
                    ProductId = dbProduct.Id,
                    ProductName = dbProduct.Name,
                    ProductSku = dbProduct.SKU,
                    ProductImage = dbProduct.Thumbnail,
                    Price = dbProduct.SalePrice > 0 ? dbProduct.SalePrice : dbProduct.BasePrice,

                    // Lưu số viên (hiển thị cho khách)
                    QuantityOnHand = cartItem.Quantity,

                    // Lưu số m2 (để kế toán kho đối soát)
                    QuantitySquareMeter = quantityM2,

                    // Tính tổng tiền (thường vẫn tính theo Giá x Số viên)
                    TotalPrice = (dbProduct.SalePrice > 0 ? dbProduct.SalePrice : dbProduct.BasePrice) * cartItem.Quantity
                };

                order.OrderItems.Add(orderItem);
                order.TotalAmount += orderItem.TotalPrice;

                // 4. TRỪ TỒN KHO THEO M2 (Quan trọng)
                if (dbProduct.ManageStock)
                {
                    // Kiểm tra tổng tồn kho hiện tại (đơn vị m2)
                    var currentStockM2 = dbProduct.Stocks.Sum(s => s.QuantityOnHand);

                    if (!dbProduct.AllowBackorder && currentStockM2 < quantityM2)
                    {
                        return Result<int>.Failure($"Sản phẩm '{dbProduct.Name}' không đủ hàng (Còn: {currentStockM2} m² - Cần: {quantityM2} m²).");
                    }

                    // Logic trừ kho
                    var m2ToDeduct = quantityM2; // Biến tạm để trừ dần

                    foreach (var stock in dbProduct.Stocks.OrderBy(s => s.QuantityOnHand)) // Ưu tiên trừ kho ít trước hoặc tùy logic
                    {
                        if (m2ToDeduct <= 0) break;

                        // stock.QuantityOnHand bây giờ là float (m2) nên trừ thoải mái
                        if (stock.QuantityOnHand >= m2ToDeduct)
                        {
                            stock.QuantityOnHand -= m2ToDeduct;
                            m2ToDeduct = 0;
                        }
                        else
                        {
                            m2ToDeduct -= stock.QuantityOnHand;
                            stock.QuantityOnHand = 0; // Hết sạch kho này
                        }
                    }
                }
            }

            try
            {
                // 6. SAVE DB
                await _unitOfWork.Repository<Order>().AddAsync(order);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // 7. XÓA GIỎ HÀNG
                await _cartRepository.DeleteCartAsync(request.CartId);

                return Result<int>.Success(order.Id);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure($"Lỗi tạo đơn hàng: {ex.Message}");
            }
        }

        private string GenerateOrderCode()
        {
            // Format: ORD-yyMMdd-Random
            return $"ORD-{DateTime.UtcNow:yyMMdd}-{new Random().Next(1000, 9999)}";
        }
    }
}