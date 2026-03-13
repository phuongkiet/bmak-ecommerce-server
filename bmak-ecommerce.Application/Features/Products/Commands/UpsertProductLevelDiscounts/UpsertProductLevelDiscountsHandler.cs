using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Entities.Identity;
using bmak_ecommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bmak_ecommerce.Application.Features.Products.Commands.UpsertProductLevelDiscounts
{
    [AutoRegister]
    public class UpsertProductLevelDiscountsHandler : ICommandHandler<UpsertProductLevelDiscountsCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpsertProductLevelDiscountsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpsertProductLevelDiscountsCommand request, CancellationToken cancellationToken = default)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.ProductId);
            if (product == null || product.IsDeleted)
                return Result<bool>.Failure("Sản phẩm không tồn tại.");

            if (request.Discounts == null || request.Discounts.Count == 0)
                return Result<bool>.Failure("Danh sách cấu hình giảm giá đang trống.");

            if (request.Discounts.Any(x => x.DiscountPercent < 0 || x.DiscountPercent > 100))
                return Result<bool>.Failure("DiscountPercent phải trong khoảng từ 0 đến 100.");

            var levelIds = request.Discounts.Select(x => x.UserLevelId).Distinct().ToList();
            var validLevelIds = await _unitOfWork.Repository<UserLevel>()
                .GetAllAsQueryable()
                .Where(x => levelIds.Contains(x.Id) && !x.IsDeleted)
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            if (validLevelIds.Count != levelIds.Count)
                return Result<bool>.Failure("Có cấp độ người dùng không hợp lệ.");

            var existing = await _unitOfWork.Repository<ProductLevelDiscount>()
                .GetAllAsQueryable()
                .Where(x => x.ProductId == request.ProductId && !x.IsDeleted)
                .ToListAsync(cancellationToken);

            foreach (var input in request.Discounts)
            {
                var current = existing.FirstOrDefault(x => x.UserLevelId == input.UserLevelId);
                if (current == null)
                {
                    await _unitOfWork.Repository<ProductLevelDiscount>().AddAsync(new ProductLevelDiscount
                    {
                        ProductId = request.ProductId,
                        UserLevelId = input.UserLevelId,
                        DiscountPercent = input.DiscountPercent,
                        IsActive = input.IsActive,
                        StartAt = input.StartAt,
                        EndAt = input.EndAt,
                        IsDeleted = false
                    });
                }
                else
                {
                    current.DiscountPercent = input.DiscountPercent;
                    current.IsActive = input.IsActive;
                    current.StartAt = input.StartAt;
                    current.EndAt = input.EndAt;
                    current.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.Repository<ProductLevelDiscount>().Update(current);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
