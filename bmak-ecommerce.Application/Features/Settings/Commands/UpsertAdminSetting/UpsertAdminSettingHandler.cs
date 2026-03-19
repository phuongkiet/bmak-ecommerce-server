using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Settings.DTOs;
using bmak_ecommerce.Domain.Entities.Settings;
using bmak_ecommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bmak_ecommerce.Application.Features.Settings.Commands.UpsertAdminSetting
{
    [AutoRegister]
    public class UpsertAdminSettingHandler : ICommandHandler<UpsertAdminSettingCommand, AdminSettingDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpsertAdminSettingHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AdminSettingDto>> Handle(UpsertAdminSettingCommand command, CancellationToken cancellationToken = default)
        {
            var repository = _unitOfWork.Repository<SiteSetting>();
            var setting = await repository.GetAllAsQueryable()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            var normalizedCompanyName = command.CompanyName.Trim();
            var normalizedSiteName = command.SiteName.Trim();
            var normalizedHotline = command.Hotline.Trim();
            var normalizedLogoUrl = command.LogoUrl.Trim();
            var normalizedTaxCode = command.TaxCode.Trim();
            var normalizedBusinessAddress = command.BusinessAddress.Trim();

            if (setting == null)
            {
                setting = new SiteSetting
                {
                    CompanyName = normalizedCompanyName,
                    SiteName = normalizedSiteName,
                    Hotline = normalizedHotline,
                    LogoUrl = normalizedLogoUrl,
                    TaxCode = normalizedTaxCode,
                    BusinessAddress = normalizedBusinessAddress
                };

                await repository.AddAsync(setting);
            }
            else
            {
                setting.CompanyName = normalizedCompanyName;
                setting.SiteName = normalizedSiteName;
                setting.Hotline = normalizedHotline;
                setting.LogoUrl = normalizedLogoUrl;
                setting.TaxCode = normalizedTaxCode;
                setting.BusinessAddress = normalizedBusinessAddress;
                setting.UpdatedAt = DateTime.UtcNow;

                repository.Update(setting);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = new AdminSettingDto
            {
                Id = setting.Id,
                CompanyName = setting.CompanyName,
                SiteName = setting.SiteName,
                Hotline = setting.Hotline,
                LogoUrl = setting.LogoUrl,
                TaxCode = setting.TaxCode,
                BusinessAddress = setting.BusinessAddress
            };

            return Result<AdminSettingDto>.Success(dto);
        }
    }
}
