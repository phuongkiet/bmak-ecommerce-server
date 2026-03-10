using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Entities.Identity;
using bmak_ecommerce.Domain.Interfaces;
using FluentValidation;

namespace bmak_ecommerce.Application.Features.Addresses.Commands.DeleteAddress
{
    [AutoRegister]
    public class DeleteAddressHandler : ICommandHandler<DeleteAddressCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<DeleteAddressCommand> _validator;

        public DeleteAddressHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IValidator<DeleteAddressCommand> validator)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _validator = validator;
        }

        public async Task<Result<bool>> Handle(DeleteAddressCommand command, CancellationToken cancellationToken = default)
        {
            if (_currentUserService.UserId <= 0)
            {
                return Result<bool>.Failure("Bạn cần đăng nhập để xoá địa chỉ.");
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
                return Result<bool>.Failure("Bạn không có quyền xoá địa chỉ này.");
            }

            address.IsDeleted = true;
            address.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
