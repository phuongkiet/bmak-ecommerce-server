using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Entities.Directory;
using bmak_ecommerce.Domain.Entities.Identity;
using bmak_ecommerce.Domain.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace bmak_ecommerce.Application.Features.Addresses.Commands.CreateAddress
{
    [AutoRegister]
    public class CreateAddressHandler : ICommandHandler<CreateAddressCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<CreateAddressCommand> _validator;

        public CreateAddressHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IValidator<CreateAddressCommand> validator)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _validator = validator;
        }

        public async Task<Result<int>> Handle(CreateAddressCommand command, CancellationToken cancellationToken = default)
        {
            if (_currentUserService.UserId <= 0)
            {
                return Result<int>.Failure("Bạn cần đăng nhập để thêm địa chỉ.");
            }

            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<int>.Failure(validationResult.Errors.First().ErrorMessage);
            }

            var normalizedProvinceId = command.ProvinceId.Trim();
            var normalizedWardId = command.WardId.Trim();

            var province = await _unitOfWork.Repository<Province>()
                .GetAllAsQueryable()
                .FirstOrDefaultAsync(x => x.Id == normalizedProvinceId, cancellationToken);
            if (province == null)
            {
                return Result<int>.Failure("Province không tồn tại.");
            }

            var ward = await _unitOfWork.Repository<Ward>()
                .GetAllAsQueryable()
                .FirstOrDefaultAsync(x => x.Id == normalizedWardId, cancellationToken);
            if (ward == null)
            {
                return Result<int>.Failure("Ward không tồn tại.");
            }

            if (!string.Equals(ward.ProvinceId, normalizedProvinceId, StringComparison.Ordinal))
            {
                return Result<int>.Failure("Ward không thuộc Province đã chọn.");
            }

            var address = new Address
            {
                ReceiverName = command.ReceiverName.Trim(),
                Phone = command.Phone.Trim(),
                Street = command.Street.Trim(),
                ProvinceId = normalizedProvinceId,
                WardId = normalizedWardId,
                Type = command.Type,
                UserId = _currentUserService.UserId
            };

            await _unitOfWork.Repository<Address>().AddAsync(address);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(address.Id);
        }
    }
}
