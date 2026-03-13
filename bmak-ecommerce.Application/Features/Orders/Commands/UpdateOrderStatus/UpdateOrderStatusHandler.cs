using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Entities.Sales;
using bmak_ecommerce.Domain.Enums;
using bmak_ecommerce.Domain.Interfaces;
using FluentValidation;

namespace bmak_ecommerce.Application.Features.Orders.Commands.UpdateOrderStatus
{
    [AutoRegister]
    public class UpdateOrderStatusHandler : ICommandHandler<UpdateOrderStatusCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<UpdateOrderStatusCommand> _validator;

        public UpdateOrderStatusHandler(
            IUnitOfWork unitOfWork,
            IValidator<UpdateOrderStatusCommand> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<Result<bool>> Handle(UpdateOrderStatusCommand command, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<bool>.Failure(validationResult.Errors.First().ErrorMessage);
            }

            var order = await _unitOfWork.Orders.GetOrderDetailAsync(command.OrderCode);
            if (order == null)
            {
                return Result<bool>.Failure("Không tìm thấy đơn hàng.");
            }

            if (order.Status == command.Status)
            {
                return Result<bool>.Failure("Đơn hàng đã ở trạng thái này.");
            }

            if (!CanTransition(order.Status, command.Status))
            {
                return Result<bool>.Failure($"Không thể chuyển trạng thái từ {order.Status} sang {command.Status}.");
            }

            order.Status = command.Status;
            order.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }

        private static bool CanTransition(OrderStatus from, OrderStatus to)
        {
            return from switch
            {
                OrderStatus.Pending => to is OrderStatus.Confirmed or OrderStatus.Cancelled,
                OrderStatus.Confirmed => to is OrderStatus.Shipping or OrderStatus.Cancelled,
                OrderStatus.Shipping => to is OrderStatus.Completed or OrderStatus.Returned,
                OrderStatus.Completed => to is OrderStatus.Returned,
                OrderStatus.Cancelled => false,
                OrderStatus.Returned => false,
                _ => false
            };
        }
    }
}
