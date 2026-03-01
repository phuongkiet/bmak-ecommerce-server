using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Entities.Identity;
using bmak_ecommerce.Domain.Interfaces;

namespace bmak_ecommerce.Application.Features.Favorites.Commands.AddFavorite
{
    [AutoRegister]
    public class AddFavoriteHandler : ICommandHandler<AddFavoriteCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public AddFavoriteHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<bool>> Handle(AddFavoriteCommand request, CancellationToken cancellationToken)
        {
            if (_currentUserService.UserId <= 0)
            {
                return Result<bool>.Failure("Bạn cần đăng nhập để sử dụng chức năng yêu thích.");
            }

            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId);
            if (product == null || !product.IsActive)
            {
                return Result<bool>.Failure("Sản phẩm không tồn tại hoặc đã ngừng kinh doanh.");
            }

            var existingFavorite = await _unitOfWork.Favorites.GetAsync(_currentUserService.UserId, request.ProductId);
            if (existingFavorite != null)
            {
                return Result<bool>.Success(true);
            }

            await _unitOfWork.Favorites.AddAsync(new UserFavoriteProduct
            {
                UserId = _currentUserService.UserId,
                ProductId = request.ProductId,
                CreatedAt = DateTime.UtcNow
            });

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
