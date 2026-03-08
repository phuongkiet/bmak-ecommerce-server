using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Entities.Sales;
using bmak_ecommerce.Domain.Enums;
using bmak_ecommerce.Domain.Interfaces;
using FluentValidation;

namespace bmak_ecommerce.Application.Features.Vouchers.Commands.CreateVoucher
{
    [AutoRegister]
    public class CreateVoucherHandler : ICommandHandler<CreateVoucherCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVoucherRepository _voucherRepository;
        private readonly IValidator<CreateVoucherCommand> _validator;

        public CreateVoucherHandler(
            IUnitOfWork unitOfWork,
            IVoucherRepository voucherRepository,
            IValidator<CreateVoucherCommand> validator)
        {
            _unitOfWork = unitOfWork;
            _voucherRepository = voucherRepository;
            _validator = validator;
        }

        public async Task<Result<int>> Handle(CreateVoucherCommand command, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<int>.Failure(validationResult.Errors.First().ErrorMessage);
            }

            var normalizedCode = command.Code.Trim().ToUpperInvariant();
            var isCodeExists = await _voucherRepository.IsCodeExistsAsync(normalizedCode);
            if (isCodeExists)
            {
                return Result<int>.Failure("Mã voucher đã tồn tại.");
            }

            var voucher = new Voucher
            {
                Code = normalizedCode,
                Description = command.Description.Trim(),
                DiscountType = command.DiscountType,
                DiscountValue = command.DiscountValue,
                MinOrderAmount = command.MinOrderAmount,
                MaxDiscountAmount = command.DiscountType == DiscountType.Percentage ? command.MaxDiscountAmount : null,
                StartDate = command.StartDate,
                EndDate = command.EndDate,
                UsageLimit = command.UsageLimit,
                UsedCount = 0,
                PerUserLimit = command.PerUserLimit,
                IsActive = command.IsActive
            };

            await _unitOfWork.Repository<Voucher>().AddAsync(voucher);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(voucher.Id);
        }
    }
}
