using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Entities.Directory;
using bmak_ecommerce.Domain.Entities.Identity;
using bmak_ecommerce.Domain.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace bmak_ecommerce.Application.Features.Addresses.Commands.UpdateAddress
{
    [AutoRegister]
    public class UpdateAddressHandler : ICommandHandler<UpdateAddressCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<UpdateAddressCommand> _validator;

        public UpdateAddressHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IValidator<UpdateAddressCommand> validator)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _validator = validator;
        }

        public async Task<Result<bool>> Handle(UpdateAddressCommand command, CancellationToken cancellationToken = default)
        {
            if (_currentUserService.UserId <= 0)
            {
                return Result<bool>.Failure("Bạn cần đăng nhập để cập nhật địa chỉ.");
            }

            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<bool>.Failure(validationResult.Errors.First().ErrorMessage);
            }

            var addressRepo = _unitOfWork.Repository<Address>();
            var address = await addressRepo.GetByIdAsync(command.Id);

            if (address == null || address.IsDeleted)
            {
                return Result<bool>.Failure("Không tìm thấy địa chỉ.");
            }

            if (address.UserId != _currentUserService.UserId)
            {
                return Result<bool>.Failure("Bạn không có quyền chỉnh sửa địa chỉ này.");
            }

            var normalizedProvinceId = command.ProvinceId.Trim();
            var normalizedWardId = command.WardId.Trim();

            var province = await _unitOfWork.Repository<Province>()
                .GetAllAsQueryable()
                .FirstOrDefaultAsync(x => x.Id == normalizedProvinceId, cancellationToken);
            if (province == null)
            {
                return Result<bool>.Failure("Province không tồn tại.");
            }

            var ward = await _unitOfWork.Repository<Ward>()
                .GetAllAsQueryable()
                .FirstOrDefaultAsync(x => x.Id == normalizedWardId, cancellationToken);
            if (ward == null)
            {
                return Result<bool>.Failure("Ward không tồn tại.");
            }

            if (!string.Equals(ward.ProvinceId, normalizedProvinceId, StringComparison.Ordinal))
            {
                return Result<bool>.Failure("Ward không thuộc Province đã chọn.");
            }

            address.ReceiverName = command.ReceiverName.Trim();
            address.Phone = command.Phone.Trim();
            address.Street = command.Street.Trim();
            address.ProvinceId = normalizedProvinceId;
            address.WardId = normalizedWardId;
            address.Type = command.Type;
            address.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
