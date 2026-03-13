using bmak_ecommerce.Domain.Enums;

namespace bmak_ecommerce.Application.Features.Orders.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusCommand
    {
        public string OrderCode { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
    }
}
