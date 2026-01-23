namespace bmak_ecommerce.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommand
    {
        public List<OrderItemDto> Items { get; set; } = new();
        public string Note { get; set; }

        // Optional – mở rộng sau
        public string CustomerName { get; set; }
        public string Address { get; set; }
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
