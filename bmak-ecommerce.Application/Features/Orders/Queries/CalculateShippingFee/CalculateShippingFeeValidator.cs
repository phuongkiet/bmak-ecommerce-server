using FluentValidation;

namespace bmak_ecommerce.Application.Features.Orders.Queries.CalculateShippingFee
{
    public class CalculateShippingFeeValidator : AbstractValidator<CalculateShippingFeeQuery>
    {
        public CalculateShippingFeeValidator()
        {
            RuleFor(x => x.CartId)
                .NotEmpty().WithMessage("CartId là bắt buộc.");

            RuleFor(x => x.Province)
                .NotEmpty().WithMessage("Tỉnh/Thành là bắt buộc.");

            RuleFor(x => x.Ward)
                .NotEmpty().WithMessage("Phường/Xã là bắt buộc.");
        }
    }
}
