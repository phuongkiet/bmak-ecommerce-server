using FluentValidation;

namespace bmak_ecommerce.Application.Features.Vouchers.Queries.ApplyVoucher
{
    public class ApplyVoucherValidator : AbstractValidator<ApplyVoucherQuery>
    {
        public ApplyVoucherValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Bạn chưa nhập mã giảm giá.")
                .MaximumLength(50).WithMessage("Code voucher không được vượt quá 50 ký tự.");

            RuleFor(x => x.CartId)
                .NotEmpty().WithMessage("Thiếu thông tin giỏ hàng.");
        }
    }
}
