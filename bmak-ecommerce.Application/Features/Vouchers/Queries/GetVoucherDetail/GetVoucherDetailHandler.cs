using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Vouchers.DTOs;
using bmak_ecommerce.Domain.Interfaces;

namespace bmak_ecommerce.Application.Features.Vouchers.Queries.GetVoucherDetail
{
    [AutoRegister]
    public class GetVoucherDetailHandler : IQueryHandler<GetVoucherDetailQuery, VoucherDto>
    {
        private readonly IVoucherRepository _voucherRepository;

        public GetVoucherDetailHandler(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }

        public async Task<Result<VoucherDto>> Handle(GetVoucherDetailQuery query, CancellationToken cancellationToken = default)
        {
            var voucher = await _voucherRepository.GetByIdAsync(query.Id);
            if (voucher == null)
            {
                return Result<VoucherDto>.Failure("Không tìm thấy voucher.");
            }

            var dto = new VoucherDto
            {
                Id = voucher.Id,
                Code = voucher.Code,
                Description = voucher.Description,
                DiscountType = voucher.DiscountType,
                DiscountValue = voucher.DiscountValue,
                MinOrderAmount = voucher.MinOrderAmount,
                MaxDiscountAmount = voucher.MaxDiscountAmount,
                StartDate = voucher.StartDate,
                EndDate = voucher.EndDate,
                UsageLimit = voucher.UsageLimit,
                UsedCount = voucher.UsedCount,
                PerUserLimit = voucher.PerUserLimit,
                IsActive = voucher.IsActive,
                CreatedAt = voucher.CreatedAt,
                UpdatedAt = voucher.UpdatedAt
            };

            return Result<VoucherDto>.Success(dto);
        }
    }
}
