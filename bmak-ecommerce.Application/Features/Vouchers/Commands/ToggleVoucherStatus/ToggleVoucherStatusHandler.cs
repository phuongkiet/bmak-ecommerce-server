using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Interfaces;
using FluentValidation;

namespace bmak_ecommerce.Application.Features.Vouchers.Commands.ToggleVoucherStatus
{
    [AutoRegister]
    public class ToggleVoucherStatusHandler : ICommandHandler<ToggleVoucherStatusCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVoucherRepository _voucherRepository;
        private readonly IValidator<ToggleVoucherStatusCommand> _validator;

        public ToggleVoucherStatusHandler(
            IUnitOfWork unitOfWork,
            IVoucherRepository voucherRepository,
            IValidator<ToggleVoucherStatusCommand> validator)
        {
            _unitOfWork = unitOfWork;
            _voucherRepository = voucherRepository;
            _validator = validator;
        }

        public async Task<Result<bool>> Handle(ToggleVoucherStatusCommand command, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<bool>.Failure(validationResult.Errors.First().ErrorMessage);
            }

            var voucher = await _voucherRepository.GetByIdAsync(command.Id);
            if (voucher == null)
            {
                return Result<bool>.Failure("Không tìm thấy voucher.");
            }

            voucher.IsActive = command.IsActive;
            voucher.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
