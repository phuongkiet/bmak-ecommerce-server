using bmak_ecommerce.Domain.Enums;
using FluentValidation;

namespace bmak_ecommerce.Application.Features.Orders.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusValidator : AbstractValidator<UpdateOrderStatusCommand>
    {
        public UpdateOrderStatusValidator()
        {
            RuleFor(x => x.OrderCode)
                .NotEmpty().WithMessage("Mã đơn hàng là bắt buộc.")
                .MaximumLength(100).WithMessage("Mã đơn hàng không hợp lệ.");

            RuleFor(x => x.Status)
                .Must(status => Enum.IsDefined(typeof(OrderStatus), status))
                .WithMessage("Trạng thái đơn hàng không hợp lệ.");
        }
    }
}
