using FluentValidation;

namespace bmak_ecommerce.Application.Features.Vouchers.Commands.ToggleVoucherStatus
{
    public class ToggleVoucherStatusValidator : AbstractValidator<ToggleVoucherStatusCommand>
    {
        public ToggleVoucherStatusValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id voucher không hợp lệ.");
        }
    }
}
