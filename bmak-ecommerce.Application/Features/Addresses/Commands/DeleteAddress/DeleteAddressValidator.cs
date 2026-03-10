using FluentValidation;

namespace bmak_ecommerce.Application.Features.Addresses.Commands.DeleteAddress
{
    public class DeleteAddressValidator : AbstractValidator<DeleteAddressCommand>
    {
        public DeleteAddressValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id địa chỉ không hợp lệ.");
        }
    }
}
