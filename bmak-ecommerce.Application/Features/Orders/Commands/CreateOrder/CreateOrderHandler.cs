using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Entities.Sales;
using bmak_ecommerce.Domain.Enums;
using bmak_ecommerce.Domain.Interfaces;

namespace bmak_ecommerce.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, Result<int>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMessageBus _messageBus;

        public CreateOrderCommandHandler(
            IUnitOfWork unitOfWork,
            IMessageBus messageBus)
        {
            _unitOfWork = unitOfWork;
            _messageBus = messageBus;
        }

        // Đổi tên hàm từ HandleAsync -> Handle cho khớp interface
        public async Task<Result<int>> Handle(
            CreateOrderCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Validate input
            if (request.Items == null || !request.Items.Any())
                return Result<int>.Failure("Giỏ hàng không được để trống");

            // 2. Load products + stocks
            var productIds = request.Items.Select(i => i.ProductId).ToList();
            var products = await _unitOfWork.Products.GetByIdsWithStocksAsync(productIds);

            if (products.Count != productIds.Distinct().Count())
                return Result<int>.Failure("Một số sản phẩm không tồn tại");

            // 3. Tạo Order
            var order = new Order
            {
                OrderCode = GenerateOrderCode(),
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                PaymentMethod = PaymentMethod.COD, // Hoặc parse từ request.PaymentMethod
                Note = request.Note,
                UserId = 1, // TODO: Lấy từ CurrentUser Service
                SubTotal = 0,
                TotalAmount = 0,
                OrderItems = new List<OrderItem>()
            };

            // 4. Xử lý từng item
            foreach (var item in request.Items)
            {
                var product = products.First(p => p.Id == item.ProductId);

                //// --- LOGIC TRỪ TỒN KHO ---
                //float totalStock = product.Stocks.Sum(s => s.QuantityOnHand);
                //if (totalStock < item.Quantity)
                //    return Result<int>.Failure($"Sản phẩm '{product.Name}' không đủ tồn kho");

                //float need = item.Quantity;
                //foreach (var stock in product.Stocks.Where(s => s.QuantityOnHand > 0).OrderByDescending(s => s.QuantityOnHand))
                //{
                //    if (need <= 0) break;
                //    if (stock.QuantityOnHand >= need)
                //    {
                //        stock.QuantityOnHand -= need;
                //        need = 0;
                //    }
                //    else
                //    {
                //        need -= stock.QuantityOnHand;
                //        stock.QuantityOnHand = 0;
                //    }
                //}

                if (product.ManageStock)
                {
                    float totalStock = product.Stocks.Sum(s => s.QuantityOnHand);

                    // Nếu không đủ tồn kho
                    if (totalStock < item.Quantity)
                    {
                        // Nếu KHÔNG cho phép đặt trước -> Chặn luôn
                        if (!product.AllowBackorder)
                        {
                            return Result<int>.Failure($"Sản phẩm '{product.Name}' tạm hết hàng.");
                        }

                        // Nếu CHO PHÉP đặt trước -> Đánh dấu đơn hàng cần verify thủ công
                        order.Status = OrderStatus.Pending; // Trạng thái mới
                    }
                }

                // --------------------------

                var price = GetValidPrice(product);

                // --- LOGIC GẠCH MEN (M2) ---
                // Tính toán diện tích: Số viên * Hệ số quy đổi
                float factor = product.ConversionFactor > 0 ? product.ConversionFactor : 1;
                float totalSquareMeter = item.Quantity * factor;

                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = price,
                    Quantity = item.Quantity,

                    // Lưu m2 vào DB
                    QuantitySquareMeter = totalSquareMeter
                };

                orderItem.TotalPrice = orderItem.UnitPrice * orderItem.Quantity;
                order.SubTotal += orderItem.TotalPrice;
                order.OrderItems.Add(orderItem);
            }

            // 5. Tính tổng tiền & Lưu DB
            order.ShippingFee = 30000;
            order.DiscountAmount = 0;
            order.TotalAmount = order.SubTotal + order.ShippingFee;

            try
            {
                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception)
            {
                return Result<int>.Failure("Không thể tạo đơn hàng");
            }

            // 6. Publish event (RabbitMQ)
            var orderEvent = new OrderCreatedEvent
            {
                OrderId = order.Id,
                OrderCode = order.OrderCode,
                TotalAmount = order.TotalAmount,
                CustomerEmail = "kietnp.ankhanh@gmail.com", // TODO: Lấy từ User
                CreatedAt = DateTime.UtcNow
            };

            await _messageBus.PublishMessageAsync(orderEvent, cancellationToken);

            return Result<int>.Success(order.Id);
        }

        private decimal GetValidPrice(Product product)
        {
            var now = DateTime.UtcNow;
            bool isSaleValid = product.SalePrice > 0 &&
                               (!product.SaleStartDate.HasValue || product.SaleStartDate <= now) &&
                               (!product.SaleEndDate.HasValue || product.SaleEndDate >= now);
            return isSaleValid ? product.SalePrice : product.BasePrice;
        }

        private string GenerateOrderCode()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}".Substring(0, 18).ToUpper();
        }
    }
}