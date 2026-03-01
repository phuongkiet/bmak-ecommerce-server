using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Favorites.Dtos;
using bmak_ecommerce.Domain.Interfaces;

namespace bmak_ecommerce.Application.Features.Favorites.Queries.GetFavorites
{
    [AutoRegister]
    public class GetFavoritesHandler : IQueryHandler<GetFavoritesQuery, List<FavoriteProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetFavoritesHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<List<FavoriteProductDto>>> Handle(GetFavoritesQuery request, CancellationToken cancellationToken)
        {
            if (_currentUserService.UserId <= 0)
            {
                return Result<List<FavoriteProductDto>>.Failure("Bạn cần đăng nhập để xem danh sách yêu thích.");
            }

            var favorites = await _unitOfWork.Favorites.GetByUserIdAsync(_currentUserService.UserId);

            var response = favorites
                .Where(x => x.Product != null && x.Product.IsActive)
                .Select(x => new FavoriteProductDto
                {
                    ProductId = x.ProductId,
                    Name = x.Product.Name,
                    Slug = x.Product.Slug,
                    Sku = x.Product.SKU,
                    Price = x.Product.SalePrice > 0 ? x.Product.SalePrice : x.Product.BasePrice,
                    OriginalPrice = x.Product.BasePrice,
                    Thumbnail = x.Product.Thumbnail ?? string.Empty,
                    AddedAt = x.CreatedAt
                })
                .ToList();

            return Result<List<FavoriteProductDto>>.Success(response);
        }
    }
}
