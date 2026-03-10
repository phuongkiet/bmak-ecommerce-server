using FluentValidation;

namespace bmak_ecommerce.Application.Features.Addresses.Commands.UpdateAddress
{
    public class UpdateAddressValidator : AbstractValidator<UpdateAddressCommand>
    {
        public UpdateAddressValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id địa chỉ không hợp lệ.");

            RuleFor(x => x.ReceiverName)
                .NotEmpty().WithMessage("Tên người nhận không được để trống.")
                .MaximumLength(120).WithMessage("Tên người nhận không được vượt quá 120 ký tự.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Số điện thoại không được để trống.")
                .MaximumLength(20).WithMessage("Số điện thoại không được vượt quá 20 ký tự.");

            RuleFor(x => x.Street)
                .NotEmpty().WithMessage("Địa chỉ đường/phố không được để trống.")
                .MaximumLength(255).WithMessage("Địa chỉ đường/phố không được vượt quá 255 ký tự.");

            RuleFor(x => x.ProvinceId)
                .NotEmpty().WithMessage("Tỉnh/Thành phố không được để trống.")
                .MaximumLength(20).WithMessage("ProvinceId không được vượt quá 20 ký tự.");

            RuleFor(x => x.WardId)
                .NotEmpty().WithMessage("Phường/Xã không được để trống.")
                .MaximumLength(20).WithMessage("WardId không được vượt quá 20 ký tự.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Loại địa chỉ không hợp lệ.");
        }
    }
}
