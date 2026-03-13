using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Entities.Identity;
using bmak_ecommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bmak_ecommerce.Application.Features.Products.Queries.Products.GetProductLevelDiscounts
{
    [AutoRegister]
    public class GetProductLevelDiscountsHandler : IQueryHandler<GetProductLevelDiscountsQuery, List<ProductLevelDiscountDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProductLevelDiscountsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<ProductLevelDiscountDto>>> Handle(GetProductLevelDiscountsQuery request, CancellationToken cancellationToken = default)
        {
            var productExists = await _unitOfWork.Repository<Product>()
                .GetAllAsQueryable()
                .AnyAsync(x => x.Id == request.ProductId && !x.IsDeleted, cancellationToken);

            if (!productExists)
                return Result<List<ProductLevelDiscountDto>>.Failure("Sản phẩm không tồn tại.");

            var result = await _unitOfWork.Repository<ProductLevelDiscount>()
                .GetAllAsQueryable()
                .Where(x => x.ProductId == request.ProductId && !x.IsDeleted)
                .Join(
                    _unitOfWork.Repository<UserLevel>().GetAllAsQueryable().Where(l => !l.IsDeleted),
                    discount => discount.UserLevelId,
                    level => level.Id,
                    (discount, level) => new ProductLevelDiscountDto
                    {
                        ProductId = discount.ProductId,
                        UserLevelId = level.Id,
                        UserLevelCode = level.Code,
                        UserLevelName = level.Name,
                        DiscountPercent = discount.DiscountPercent,
                        IsActive = discount.IsActive,
                        StartAt = discount.StartAt,
                        EndAt = discount.EndAt
                    })
                .OrderBy(x => x.UserLevelId)
                .ToListAsync(cancellationToken);

            return Result<List<ProductLevelDiscountDto>>.Success(result);
        }
    }
}
