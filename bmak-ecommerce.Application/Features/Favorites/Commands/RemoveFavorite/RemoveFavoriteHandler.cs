using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Interfaces;

namespace bmak_ecommerce.Application.Features.Favorites.Commands.RemoveFavorite
{
    [AutoRegister]
    public class RemoveFavoriteHandler : ICommandHandler<RemoveFavoriteCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public RemoveFavoriteHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<bool>> Handle(RemoveFavoriteCommand request, CancellationToken cancellationToken)
        {
            if (_currentUserService.UserId <= 0)
            {
                return Result<bool>.Failure("Bạn cần đăng nhập để sử dụng chức năng yêu thích.");
            }

            var favorite = await _unitOfWork.Favorites.GetAsync(_currentUserService.UserId, request.ProductId);

            if (favorite == null)
            {
                return Result<bool>.Success(true);
            }

            _unitOfWork.Favorites.Remove(favorite);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
