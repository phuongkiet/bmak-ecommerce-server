using bmak_ecommerce.Application.Features.Products.DTOs.Sale;

namespace bmak_ecommerce.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommand
    {
        public List<OrderItemDto> Items { get; set; } = new(); // DTO đầu vào từ Cart
        public string Note { get; set; }
        public string? ShippingAddress { get; set; }
        public string PaymentMethod { get; set; }
        // ... các trường khác nếu có
    }
}
