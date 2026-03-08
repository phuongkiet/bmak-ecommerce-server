using FluentValidation;

namespace bmak_ecommerce.Application.Features.Vouchers.Commands.DeleteVoucher
{
    public class DeleteVoucherValidator : AbstractValidator<DeleteVoucherCommand>
    {
        public DeleteVoucherValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id voucher không hợp lệ.");
        }
    }
}
