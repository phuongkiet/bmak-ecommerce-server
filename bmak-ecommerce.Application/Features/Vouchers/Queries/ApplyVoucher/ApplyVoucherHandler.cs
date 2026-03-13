using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Vouchers.DTOs;
using bmak_ecommerce.Domain.Entities.Sales;
using bmak_ecommerce.Domain.Enums;
using bmak_ecommerce.Domain.Interfaces;
using FluentValidation;

namespace bmak_ecommerce.Application.Features.Vouchers.Queries.ApplyVoucher
{
    [AutoRegister]
    public class ApplyVoucherHandler : IQueryHandler<ApplyVoucherQuery, VoucherResponseDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartRepository _cartRepository;
        private readonly IVoucherRepository _voucherRepository;
        private readonly IValidator<ApplyVoucherQuery> _validator;
        private readonly ICurrentUserService _currentUserService;

        public ApplyVoucherHandler(
            IUnitOfWork unitOfWork,
            ICartRepository cartRepository,
            IVoucherRepository voucherRepository,
            IValidator<ApplyVoucherQuery> validator,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _cartRepository = cartRepository;
            _voucherRepository = voucherRepository;
            _validator = validator;
            _currentUserService = currentUserService;
        }

        public async Task<Result<VoucherResponseDto>> Handle(ApplyVoucherQuery request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<VoucherResponseDto>.Failure(validationResult.Errors.First().ErrorMessage);
            }

            // 1. TÌM VÀ KIỂM TRA MÃ VOUCHER
            var voucher = await _voucherRepository.GetByCodeAsync(request.Code);

            if (voucher == null)
                return Result<VoucherResponseDto>.Failure("Mã giảm giá không tồn tại.");

            if (!voucher.IsValid())
                return Result<VoucherResponseDto>.Failure("Mã giảm giá đã hết hạn, bị khóa hoặc hết lượt sử dụng.");

            // 2. LẤY GIỎ HÀNG ĐỂ TÍNH TỔNG TIỀN (Chống FE gửi tiền láo)
            var effectiveCartId = ResolveEffectiveCartId(request.CartId);
            var cart = await _cartRepository.GetCartAsync(effectiveCartId);
            if (cart == null || !cart.Items.Any())
                return Result<VoucherResponseDto>.Failure("Giỏ hàng trống.");

            var productIds = cart.Items.Select(x => x.ProductId).ToList();
            var dbProducts = await _unitOfWork.Products.GetByIdsAsync(productIds);

            decimal subTotal = 0;
            foreach (var item in cart.Items)
            {
                var p = dbProducts.FirstOrDefault(x => x.Id == item.ProductId);
                if (p != null)
                {
                    decimal price = p.SalePrice > 0 ? p.SalePrice : p.BasePrice;
                    subTotal += price * item.Quantity;
                }
            }

            // 3. KIỂM TRA ĐIỀU KIỆN TỔNG TIỀN
            if (subTotal < voucher.MinOrderAmount)
                return Result<VoucherResponseDto>.Failure($"Đơn hàng chưa đạt mức tối thiểu {voucher.MinOrderAmount:N0}đ để áp dụng mã này.");

            // 4. TÍNH TOÁN SỐ TIỀN ĐƯỢC GIẢM
            decimal discountAmount = 0;

            if (voucher.DiscountType == DiscountType.FixedAmount)
            {
                discountAmount = voucher.DiscountValue;
            }
            else if (voucher.DiscountType == DiscountType.Percentage)
            {
                discountAmount = subTotal * (voucher.DiscountValue / 100);

                // Nếu có giới hạn mức giảm tối đa
                if (voucher.MaxDiscountAmount.HasValue && discountAmount > voucher.MaxDiscountAmount.Value)
                {
                    discountAmount = voucher.MaxDiscountAmount.Value;
                }
            }

            // Không để tiền giảm giá lớn hơn cả tiền hàng
            if (discountAmount > subTotal) discountAmount = subTotal;

            // 5. TRẢ KẾT QUẢ VỀ CHO FRONTEND
            var finalTotal = subTotal - discountAmount;

            return Result<VoucherResponseDto>.Success(new VoucherResponseDto
            {
                Code = voucher.Code,
                DiscountAmount = discountAmount,
                Message = $"Áp dụng mã thành công. Bạn được giảm {discountAmount:N0}đ, còn lại {finalTotal:N0}đ."
            });
        }

        private string ResolveEffectiveCartId(string? clientCartId)
        {
            if (_currentUserService.UserId > 0)
            {
                return $"cart:user:{_currentUserService.UserId}";
            }

            return clientCartId ?? string.Empty;
        }
    }
}
