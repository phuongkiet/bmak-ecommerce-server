using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Orders.Models;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Entities.Identity;
using bmak_ecommerce.Domain.Entities.Sales;
using bmak_ecommerce.Domain.Enums;
using bmak_ecommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Orders.Commands.CreateOrder
{
    [AutoRegister]
    public class CreateOrderHandler : ICommandHandler<CreateOrderCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartRepository _cartRepository; // Service lấy giỏ hàng (Redis/DB)
        private readonly ICurrentUserService _currentUserService; // Lấy UserId đăng nhập
        private readonly IMessageBus _messageBus; // Service bắn sự kiện RabbitMQ
        private readonly IShippingRuleEngine _shippingRuleEngine;
        private readonly UserManager<AppUser> _userManager;

        public CreateOrderHandler(
            IUnitOfWork unitOfWork,
            ICartRepository cartRepository,
            ICurrentUserService currentUserService,
            IMessageBus messageBus,
            IShippingRuleEngine shippingRuleEngine,
            UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _cartRepository = cartRepository;
            _currentUserService = currentUserService;
            _messageBus = messageBus;
            _shippingRuleEngine = shippingRuleEngine;
            _userManager = userManager;
        }

        public async Task<Result<int>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // 1. LẤY GIỎ HÀNG
            var effectiveCartId = ResolveEffectiveCartId(request.CartId);
            var cart = await _cartRepository.GetCartAsync(effectiveCartId);

            if (cart == null || !cart.Items.Any())
            {
                return Result<int>.Failure("Giỏ hàng trống hoặc không tồn tại.");
            }

            // 2. KIỂM TRA TỒN KHO & GIÁ (Validate Stock)
            var productIds = cart.Items.Select(x => x.ProductId).ToList();
            var dbProducts = await _unitOfWork.Products.GetByIdsAsync(productIds);

            int? userLevelId = null;
            var currentUserId = _currentUserService.UserId;
            if (currentUserId > 0)
            {
                userLevelId = await _userManager.Users
                    .Where(x => x.Id == currentUserId)
                    .Select(x => x.UserLevelId)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            var activeLevelDiscounts = new Dictionary<int, decimal>();
            if (userLevelId.HasValue)
            {
                var now = DateTime.UtcNow;
                activeLevelDiscounts = await _unitOfWork.Repository<ProductLevelDiscount>()
                    .GetAllAsQueryable()
                    .Where(x =>
                        !x.IsDeleted &&
                        x.IsActive &&
                        x.UserLevelId == userLevelId.Value &&
                        productIds.Contains(x.ProductId) &&
                        (!x.StartAt.HasValue || x.StartAt.Value <= now) &&
                        (!x.EndAt.HasValue || x.EndAt.Value >= now))
                    .ToDictionaryAsync(x => x.ProductId, x => x.DiscountPercent, cancellationToken);
            }

            foreach (var cartItem in cart.Items)
            {
                var dbProduct = dbProducts.FirstOrDefault(p => p.Id == cartItem.ProductId);

                if (dbProduct == null)
                    return Result<int>.Failure($"Sản phẩm ID {cartItem.ProductId} không còn tồn tại.");

                if (!dbProduct.IsActive)
                    return Result<int>.Failure($"Sản phẩm '{dbProduct.Name}' đang ngừng kinh doanh.");

                // Check số lượng tồn (Tổng số VIÊN của tất cả các kho)
                int currentStockPieces = dbProduct.Stocks.Sum(s => s.QuantityOnHand);

                // Nếu có quản lý kho và không cho bán âm
                if (!UsesSoftStock(dbProduct) && dbProduct.ManageStock && !dbProduct.AllowBackorder && currentStockPieces < cartItem.Quantity)
                {
                    return Result<int>.Failure($"Sản phẩm '{dbProduct.Name}' không đủ hàng (Còn: {currentStockPieces}).");
                }
            }

            // 3. XỬ LÝ LOGIC ĐỊA CHỈ (Billing vs Shipping)
            string finalReceiverName, finalReceiverPhone, finalShippingAddress;
            string finalBillingAddress = request.BillingAddress.ToString();

            if (request.ShipToDifferentAddress)
            {
                if (request.ShippingAddress == null)
                    return Result<int>.Failure("Chưa nhập địa chỉ giao hàng.");

                finalReceiverName = request.ReceiverName;
                finalReceiverPhone = request.ReceiverPhone;
                finalShippingAddress = request.ShippingAddress.ToString();
            }
            else
            {
                finalReceiverName = request.BuyerName;
                finalReceiverPhone = request.BuyerPhone;
                finalShippingAddress = finalBillingAddress;
            }

            var shippingProvince = request.ShipToDifferentAddress
                ? request.ShippingAddress?.Province ?? string.Empty
                : request.BillingAddress.Province;

            var shippingWard = request.ShipToDifferentAddress
                ? request.ShippingAddress?.Ward ?? string.Empty
                : request.BillingAddress.Ward;

            var shippingContext = BuildShippingRuleContext(cart, dbProducts, shippingProvince, shippingWard, activeLevelDiscounts);
            var shippingResult = await _shippingRuleEngine.CalculateAsync(shippingContext, cancellationToken);

            // 4. TẠO ORDER ENTITY
            var order = new Order
            {
                OrderCode = GenerateOrderCode(),
                Status = OrderStatus.Pending,
                PaymentMethod = request.PaymentMethod,
                Note = request.Note,
                OrderDate = DateTime.UtcNow,
                UserId = _currentUserService.UserId != 0 ? _currentUserService.UserId : 0, // Gán null nếu là khách vãng lai (Guest)

                BuyerName = request.BuyerName,
                BuyerPhone = request.BuyerPhone,
                BuyerEmail = request.BuyerEmail,
                BillingAddress = finalBillingAddress,

                ReceiverName = finalReceiverName,
                ReceiverPhone = finalReceiverPhone,
                ShippingAddress = finalShippingAddress,

                // Luôn tính lại từ rule, không trust dữ liệu FE.
                ShippingFee = shippingResult.ShippingFee,
                DiscountAmount = request.DiscountAmount,

                // Khởi tạo các giá trị tiền, sẽ được cộng dồn ở phần lặp Items
                SubTotal = 0,
                TotalAmount = 0
            };

            // 5. TẠO ORDER ITEMS & TRỪ KHO
            foreach (var cartItem in cart.Items)
            {
                var dbProduct = dbProducts.FirstOrDefault(p => p.Id == cartItem.ProductId);

                // Tính toán hệ số m2 (Ép kiểu float)
                float factor = dbProduct.ConversionFactor > 0 ? dbProduct.ConversionFactor : 1;
                float quantityM2 = (float)cartItem.Quantity * factor;

                var orderItem = new OrderItem
                {
                    ProductId = dbProduct.Id,
                    ProductName = dbProduct.Name,
                    ProductSku = dbProduct.SKU,
                    ProductImage = dbProduct.Thumbnail,
                    Price = CalculateFinalUnitPrice(dbProduct, activeLevelDiscounts),

                    // BẮT BUỘC KIỂU INT: Số viên khách mua
                    QuantityOnHand = cartItem.Quantity,

                    // KIỂU FLOAT: Số m2 quy đổi
                    QuantitySquareMeter = quantityM2,

                    TotalPrice = CalculateFinalUnitPrice(dbProduct, activeLevelDiscounts) * cartItem.Quantity
                };

                order.OrderItems.Add(orderItem);

                // Cộng dồn vào TỔNG TIỀN HÀNG (SubTotal)
                order.SubTotal += orderItem.TotalPrice;

                // TRỪ TỒN KHO THEO SỐ VIÊN (INT)
                if (!UsesSoftStock(dbProduct) && dbProduct.ManageStock)
                {
                    int piecesToDeduct = cartItem.Quantity;

                    // Ưu tiên kho có ít hàng trừ trước, hoặc tùy rule của sếp
                    foreach (var stock in dbProduct.Stocks.OrderBy(s => s.QuantityOnHand))
                    {
                        if (piecesToDeduct <= 0) break; // Đã trừ đủ số lượng

                        if (stock.QuantityOnHand >= piecesToDeduct)
                        {
                            stock.QuantityOnHand -= piecesToDeduct;
                            piecesToDeduct = 0;
                        }
                        else
                        {
                            piecesToDeduct -= stock.QuantityOnHand;
                            stock.QuantityOnHand = 0; // Trừ cạn kho này, chuyển sang kho kế tiếp
                        }
                    }
                }
            }

            // TÍNH TỔNG TIỀN CUỐI CÙNG
            // Công thức chuẩn: Tổng thanh toán = Tiền hàng + Ship - Giảm giá
            order.TotalAmount = order.SubTotal + order.ShippingFee - order.DiscountAmount;

            // Không để tổng tiền bị âm (Đề phòng mã giảm giá xịn quá)
            if (order.TotalAmount < 0) order.TotalAmount = 0;

            try
            {
                // 6. SAVE DATABASE
                await _unitOfWork.Repository<Order>().AddAsync(order);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // 7. XÓA GIỎ HÀNG SAU KHI ĐẶT THÀNH CÔNG
                await _cartRepository.DeleteCartAsync(effectiveCartId);

                // 8. BẮN EVENT SANG RABBITMQ ĐỂ GỬI MAIL & PUSH THÔNG BÁO
                var orderEvent = new OrderCreatedEvent
                {
                    OrderId = order.Id,
                    OrderCode = order.OrderCode,
                    TotalAmount = order.TotalAmount,
                    CustomerEmail = request.BuyerEmail,
                    CreatedAt = DateTime.UtcNow,

                    // Nạp danh sách món hàng vào gói tin để in ra bảng trong Email
                    Items = order.OrderItems.Select(x => new OrderItemMessage
                    {
                        ProductName = x.ProductName,
                        Quantity = x.QuantityOnHand, // Số viên
                        Price = x.Price,
                        TotalPrice = x.TotalPrice
                    }).ToList()
                };
                await _messageBus.PublishMessageAsync(orderEvent, cancellationToken);

                return Result<int>.Success(order.Id);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure($"Lỗi tạo đơn hàng: {ex.Message}");
            }
        }

        private static bool UsesSoftStock(Product product)
        {
            return product.WordPressProductId.HasValue || !product.ManageStock || product.AllowBackorder;
        }

        private string ResolveEffectiveCartId(string? clientCartId)
        {
            if (_currentUserService.UserId > 0)
            {
                return $"cart:user:{_currentUserService.UserId}";
            }

            return clientCartId ?? string.Empty;
        }

        private string GenerateOrderCode()
        {
            // Format: ORD-yyMMdd-Random (Ví dụ: ORD-231025-5106)
            return $"ORD-{DateTime.UtcNow:yyMMdd}-{new Random().Next(1000, 9999)}";
        }

        private static decimal CalculateFinalUnitPrice(Product product, IReadOnlyDictionary<int, decimal> activeLevelDiscounts)
        {
            var basePrice = product.SalePrice > 0 ? product.SalePrice : product.BasePrice;

            if (!activeLevelDiscounts.TryGetValue(product.Id, out var percent) || percent <= 0)
                return basePrice;

            var finalPrice = basePrice * (1 - (percent / 100m));
            if (finalPrice < 0) finalPrice = 0;

            return Math.Round(finalPrice, 2, MidpointRounding.AwayFromZero);
        }

        private static ShippingRuleContext BuildShippingRuleContext(
            bmak_ecommerce.Application.Features.Cart.Models.ShoppingCart cart,
            List<bmak_ecommerce.Domain.Entities.Catalog.Product> dbProducts,
            string province,
            string ward,
            IReadOnlyDictionary<int, decimal> activeLevelDiscounts)
        {
            decimal subTotal = 0;
            decimal totalWeight = 0;
            decimal totalSquareMeter = 0;
            int itemCount = 0;

            foreach (var item in cart.Items)
            {
                var product = dbProducts.FirstOrDefault(x => x.Id == item.ProductId);
                if (product == null)
                {
                    continue;
                }

                var unitPrice = CalculateFinalUnitPrice(product, activeLevelDiscounts);
                var conversionFactor = product.ConversionFactor > 0 ? (decimal)product.ConversionFactor : 1m;
                var productWeight = product.Weight > 0 ? (decimal)product.Weight : 0m;

                subTotal += unitPrice * item.Quantity;
                totalSquareMeter += conversionFactor * item.Quantity;
                totalWeight += productWeight * item.Quantity;
                itemCount += item.Quantity;
            }

            return new ShippingRuleContext
            {
                SubTotal = subTotal,
                TotalWeight = totalWeight,
                TotalSquareMeter = totalSquareMeter,
                ItemCount = itemCount,
                Province = province,
                Ward = ward
            };
        }
    }
}