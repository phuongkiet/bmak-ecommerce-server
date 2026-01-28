using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Entities.Sales;
using bmak_ecommerce.Domain.Enums;
using bmak_ecommerce.Domain.Interfaces;

namespace bmak_ecommerce.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderHandler : ICommandHandler<CreateOrderCommand, Result<int>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartRepository _cartRepository; // Inject thêm cái này
        private readonly IMessageBus _messageBus;
        //private readonly ICurrentUserService _currentUserService;

        public CreateOrderHandler(
            IUnitOfWork unitOfWork,
            ICartRepository cartRepository,
            IMessageBus messageBus
            //ICurrentUserService currentUserService
            )
        {
            _unitOfWork = unitOfWork;
            _cartRepository = cartRepository;
            _messageBus = messageBus;
            //_currentUserService = currentUserService;
        }

        public async Task<Result<int>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // 1. Lấy Giỏ hàng từ Redis
            var cart = await _cartRepository.GetCartAsync(request.CartId);
            if (cart == null || !cart.Items.Any())
            {
                return Result<int>.Failure("Giỏ hàng trống hoặc đã hết hạn.");
            }

            // 2. Lấy thông tin sản phẩm mới nhất từ DB (Để check giá & tồn kho chuẩn)
            var productIds = cart.Items.Select(i => i.ProductId).ToList();
            var products = await _unitOfWork.Products.GetByIdsWithStocksAsync(productIds);

            string finalReceiverName;
            string finalReceiverPhone;
            string finalShippingAddress;
            string finalBillingAddress = request.BillingAddress.ToString(); // Helper method trong DTO

            if (request.ShipToDifferentAddress)
            {
                // CASE A: Giao tới địa chỉ khác (Công trình)
                // Bắt buộc phải có thông tin Shipping gửi lên
                if (request.ShippingAddress == null || string.IsNullOrEmpty(request.ReceiverName))
                {
                    return Result<int>.Failure("Vui lòng nhập đầy đủ thông tin người nhận hàng.");
                }

                finalReceiverName = request.ReceiverName;
                finalReceiverPhone = request.ReceiverPhone;
                finalShippingAddress = request.ShippingAddress.ToString();
            }
            else
            {
                // CASE B: Giao tới địa chỉ người mua (Mặc định)
                // Copy từ Billing sang
                finalReceiverName = request.BuyerName;
                finalReceiverPhone = request.BuyerPhone;
                finalShippingAddress = finalBillingAddress;
            }

            // 3. Khởi tạo Order
            var order = new Order
            {
                OrderCode = GenerateOrderCode(),
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                PaymentMethod = ParsePaymentMethod(request.PaymentMethod),
                Note = request.Note,
                UserId = 1,
                //UserId = _currentUserService.UserId != 0 ? _currentUserService.UserId : 1,

                // --- MAPPING SNAPSHOT ---
                // Thông tin người mua
                BuyerName = request.BuyerName,
                BuyerPhone = request.BuyerPhone,
                BuyerEmail = request.BuyerEmail,
                BillingAddress = finalBillingAddress,

                // Thông tin giao hàng (Đã xử lý logic ở trên)
                ReceiverName = finalReceiverName,
                ReceiverPhone = finalReceiverPhone,
                ShippingAddress = finalShippingAddress,
                // ------------------------

                OrderItems = new List<OrderItem>()
            };

            bool needsConfirmation = false; // Cờ đánh dấu đơn hàng cần xác nhận thủ công

            // 4. Duyệt từng item trong Cart
            foreach (var cartItem in cart.Items)
            {
                var product = products.FirstOrDefault(p => p.Id == cartItem.ProductId);
                if (product == null) continue; // Skip nếu sản phẩm bị xóa

                // --- LOGIC GIÁ ---
                // Luôn lấy giá từ DB để bảo mật (tránh hack giá từ Redis/Frontend)
                var currentPrice = product.SalePrice > 0 ? product.SalePrice : product.BasePrice;

                // --- LOGIC CHECK TỒN KHO & SOFT STOCK ---
                if (product.ManageStock)
                {
                    float totalStock = product.Stocks.Sum(s => s.QuantityOnHand);

                    // Nếu thiếu hàng
                    if (totalStock < cartItem.Quantity)
                    {
                        if (!product.AllowBackorder)
                        {
                            return Result<int>.Failure($"Sản phẩm '{product.Name}' hiện đã hết hàng.");
                        }

                        // Cho phép đặt nhưng đánh dấu cần xác nhận
                        needsConfirmation = true;
                    }

                    // Trừ tồn kho (Best effort)
                    DeductStock(product, cartItem.Quantity);
                }

                // --- TẠO ORDER ITEM ---
                // Tính lại m2 chuẩn từ DB
                float factor = product.ConversionFactor > 0 ? product.ConversionFactor : 1;

                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = currentPrice,
                    Quantity = cartItem.Quantity,

                    // Logic m2 (Gạch men)
                    QuantitySquareMeter = cartItem.Quantity * factor,

                    TotalPrice = currentPrice * cartItem.Quantity
                };

                order.SubTotal += orderItem.TotalPrice;
                order.OrderItems.Add(orderItem);
            }

            // Nếu có sản phẩm backorder, chuyển trạng thái đơn hàng
            if (needsConfirmation)
            {
                order.Status = OrderStatus.Pending; // "Chờ xác nhận"
            }

            // 5. Tính tổng tiền cuối cùng
            order.ShippingFee = 0; // Có thể tính phí ship sau
            order.TotalAmount = order.SubTotal + order.ShippingFee;

            try
            {
                // Lưu Order
                await _unitOfWork.Orders.AddAsync(order);

                // Lưu thay đổi Tồn kho (nếu có)
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // 6. QUAN TRỌNG: Xóa Cart sau khi đặt thành công
                await _cartRepository.DeleteCartAsync(request.CartId);
            }
            catch (Exception ex)
            {
                // Log error
                return Result<int>.Failure("Lỗi khi tạo đơn hàng: " + ex.Message);
            }

            // 7. Gửi Event (RabbitMQ)
            var orderEvent = new OrderCreatedEvent
            {
                OrderId = order.Id,
                OrderCode = order.OrderCode,
                TotalAmount = order.TotalAmount,
                CustomerEmail = "customer@example.com",
                CreatedAt = DateTime.UtcNow
            };
            await _messageBus.PublishMessageAsync(orderEvent, cancellationToken);

            return Result<int>.Success(order.Id);
        }

        // Helper: Trừ tồn kho ưu tiên lô cũ trước (FIFO đơn giản) hoặc kho nhiều hàng nhất
        private void DeductStock(Domain.Entities.Catalog.Product product, int quantityNeeded)
        {
            var stocks = product.Stocks.OrderByDescending(s => s.QuantityOnHand).ToList();
            float remain = quantityNeeded;

            foreach (var stock in stocks)
            {
                if (remain <= 0) break;

                if (stock.QuantityOnHand >= remain)
                {
                    stock.QuantityOnHand -= remain;
                    remain = 0;
                }
                else
                {
                    remain -= stock.QuantityOnHand;
                    stock.QuantityOnHand = 0;
                }
            }
            // Lưu ý: Nếu remain > 0 (bán âm), số dư nợ sẽ không trừ vào DB 
            // mà chỉ ghi nhận Order để nhập hàng sau.
        }

        private string GenerateOrderCode()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
        }

        private PaymentMethod ParsePaymentMethod(string method)
        {
            return method?.ToUpper() == "BANK" ? PaymentMethod.BankTransfer : PaymentMethod.COD;
        }
    }
}