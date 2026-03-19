using FluentValidation;

namespace bmak_ecommerce.Application.Features.Settings.Commands.UpsertAdminSetting
{
    public class UpsertAdminSettingValidator : AbstractValidator<UpsertAdminSettingCommand>
    {
        public UpsertAdminSettingValidator()
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("Ten doanh nghiep la bat buoc.")
                .MaximumLength(255).WithMessage("Ten doanh nghiep toi da 255 ky tu.");

            RuleFor(x => x.SiteName)
                .NotEmpty().WithMessage("Ten trang la bat buoc.")
                .MaximumLength(255).WithMessage("Ten trang toi da 255 ky tu.");

            RuleFor(x => x.Hotline)
                .NotEmpty().WithMessage("So hotline la bat buoc.")
                .MaximumLength(50).WithMessage("So hotline toi da 50 ky tu.");

            RuleFor(x => x.LogoUrl)
                .MaximumLength(1000).WithMessage("Logo URL toi da 1000 ky tu.");

            RuleFor(x => x.TaxCode)
                .NotEmpty().WithMessage("Ma so thue la bat buoc.")
                .MaximumLength(50).WithMessage("Ma so thue toi da 50 ky tu.");

            RuleFor(x => x.BusinessAddress)
                .NotEmpty().WithMessage("Dia chi kinh doanh la bat buoc.")
                .MaximumLength(500).WithMessage("Dia chi kinh doanh toi da 500 ky tu.");
        }
    }
}
