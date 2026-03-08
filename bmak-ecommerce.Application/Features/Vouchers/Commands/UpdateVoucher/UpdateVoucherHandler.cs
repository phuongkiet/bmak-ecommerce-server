using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Enums;
using bmak_ecommerce.Domain.Interfaces;
using FluentValidation;

namespace bmak_ecommerce.Application.Features.Vouchers.Commands.UpdateVoucher
{
    [AutoRegister]
    public class UpdateVoucherHandler : ICommandHandler<UpdateVoucherCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVoucherRepository _voucherRepository;
        private readonly IValidator<UpdateVoucherCommand> _validator;

        public UpdateVoucherHandler(
            IUnitOfWork unitOfWork,
            IVoucherRepository voucherRepository,
            IValidator<UpdateVoucherCommand> validator)
        {
            _unitOfWork = unitOfWork;
            _voucherRepository = voucherRepository;
            _validator = validator;
        }

        public async Task<Result<bool>> Handle(UpdateVoucherCommand command, CancellationToken cancellationToken = default)
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

            var normalizedCode = command.Code.Trim().ToUpperInvariant();
            var isDuplicateCode = await _voucherRepository.IsCodeExistsAsync(normalizedCode, command.Id);
            if (isDuplicateCode)
            {
                return Result<bool>.Failure("Mã voucher đã tồn tại.");
            }

            voucher.Code = normalizedCode;
            voucher.Description = command.Description.Trim();
            voucher.DiscountType = command.DiscountType;
            voucher.DiscountValue = command.DiscountValue;
            voucher.MinOrderAmount = command.MinOrderAmount;
            voucher.MaxDiscountAmount = command.DiscountType == DiscountType.Percentage ? command.MaxDiscountAmount : null;
            voucher.StartDate = command.StartDate;
            voucher.EndDate = command.EndDate;
            voucher.UsageLimit = command.UsageLimit;
            voucher.PerUserLimit = command.PerUserLimit;
            voucher.IsActive = command.IsActive;
            voucher.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
