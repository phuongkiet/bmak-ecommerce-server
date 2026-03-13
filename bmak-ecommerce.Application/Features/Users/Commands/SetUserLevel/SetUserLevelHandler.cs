using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Entities.Identity;
using bmak_ecommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace bmak_ecommerce.Application.Features.Users.Commands.SetUserLevel
{
    [AutoRegister]
    public class SetUserLevelHandler : ICommandHandler<SetUserLevelCommand, bool>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public SetUserLevelHandler(UserManager<AppUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(SetUserLevelCommand request, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                return Result<bool>.Failure("Không tìm thấy người dùng.");

            var level = await _unitOfWork.Repository<UserLevel>().GetByIdAsync(request.UserLevelId);
            if (level == null || level.IsDeleted || !level.IsActive)
                return Result<bool>.Failure("Cấp độ không tồn tại hoặc đã bị khóa.");

            user.UserLevelId = level.Id;
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
                return Result<bool>.Failure(string.Join(", ", updateResult.Errors.Select(x => x.Description)));

            return Result<bool>.Success(true);
        }
    }
}
