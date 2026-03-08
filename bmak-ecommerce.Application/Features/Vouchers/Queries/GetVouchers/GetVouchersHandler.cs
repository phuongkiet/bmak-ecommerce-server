using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Vouchers.DTOs;
using bmak_ecommerce.Domain.Entities.Sales;
using bmak_ecommerce.Domain.Interfaces;
using bmak_ecommerce.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace bmak_ecommerce.Application.Features.Vouchers.Queries.GetVouchers
{
    [AutoRegister]
    public class GetVouchersHandler : IQueryHandler<GetVouchersQuery, PagedList<VoucherDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetVouchersHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagedList<VoucherDto>>> Handle(GetVouchersQuery query, CancellationToken cancellationToken = default)
        {
            var specParams = query.Params;
            var pageIndex = specParams.PageIndex < 1 ? 1 : specParams.PageIndex;
            var pageSize = specParams.PageSize < 1 ? 10 : specParams.PageSize;

            var voucherQuery = _unitOfWork.Repository<Voucher>()
                .GetAllAsQueryable()
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(specParams.Search))
            {
                var keyword = specParams.Search.Trim().ToLower();
                voucherQuery = voucherQuery.Where(x =>
                    x.Code.ToLower().Contains(keyword)
                    || x.Description.ToLower().Contains(keyword));
            }

            if (specParams.IsActive.HasValue)
            {
                voucherQuery = voucherQuery.Where(x => x.IsActive == specParams.IsActive.Value);
            }

            var totalCount = await voucherQuery.CountAsync(cancellationToken);

            var vouchers = await voucherQuery
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new VoucherDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Description = x.Description,
                    DiscountType = x.DiscountType,
                    DiscountValue = x.DiscountValue,
                    MinOrderAmount = x.MinOrderAmount,
                    MaxDiscountAmount = x.MaxDiscountAmount,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    UsageLimit = x.UsageLimit,
                    UsedCount = x.UsedCount,
                    PerUserLimit = x.PerUserLimit,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .ToListAsync(cancellationToken);

            var pagedResult = new PagedList<VoucherDto>(
                vouchers,
                totalCount,
                pageIndex,
                pageSize);

            return Result<PagedList<VoucherDto>>.Success(pagedResult);
        }
    }
}
