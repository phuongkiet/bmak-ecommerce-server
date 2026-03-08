using bmak_ecommerce.Domain.Enums;
using FluentValidation;

namespace bmak_ecommerce.Application.Features.Vouchers.Commands.CreateVoucher
{
    public class CreateVoucherValidator : AbstractValidator<CreateVoucherCommand>
    {
        public CreateVoucherValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code voucher không được để trống.")
                .MaximumLength(50).WithMessage("Code voucher không được vượt quá 50 ký tự.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Mô tả voucher không được để trống.")
                .MaximumLength(500).WithMessage("Mô tả voucher không được vượt quá 500 ký tự.");

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .WithMessage("Ngày kết thúc phải lớn hơn ngày bắt đầu.");

            RuleFor(x => x.DiscountValue)
                .GreaterThan(0).WithMessage("Giá trị giảm phải lớn hơn 0.");

            RuleFor(x => x.DiscountValue)
                .InclusiveBetween(0.01m, 100m)
                .When(x => x.DiscountType == DiscountType.Percentage)
                .WithMessage("Voucher theo phần trăm chỉ cho phép DiscountValue từ 0 đến 100.");

            RuleFor(x => x.MinOrderAmount)
                .GreaterThanOrEqualTo(0).WithMessage("MinOrderAmount không được nhỏ hơn 0.");

            RuleFor(x => x.UsageLimit)
                .GreaterThanOrEqualTo(0).WithMessage("UsageLimit không được nhỏ hơn 0.");

            RuleFor(x => x.PerUserLimit)
                .GreaterThan(0).WithMessage("PerUserLimit phải lớn hơn 0.");

            RuleFor(x => x.MaxDiscountAmount)
                .GreaterThan(0).When(x => x.MaxDiscountAmount.HasValue)
                .WithMessage("MaxDiscountAmount phải lớn hơn 0 nếu được khai báo.");

            RuleFor(x => x.MaxDiscountAmount)
                .Null().When(x => x.DiscountType == DiscountType.FixedAmount)
                .WithMessage("MaxDiscountAmount chỉ dùng cho voucher phần trăm.");
        }
    }
}
