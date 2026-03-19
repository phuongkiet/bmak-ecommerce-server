using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Settings.DTOs;
using bmak_ecommerce.Domain.Entities.Settings;
using bmak_ecommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bmak_ecommerce.Application.Features.Settings.Queries.GetAdminSetting
{
    [AutoRegister]
    public class GetAdminSettingHandler : IQueryHandler<GetAdminSettingQuery, AdminSettingDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAdminSettingHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AdminSettingDto>> Handle(GetAdminSettingQuery query, CancellationToken cancellationToken = default)
        {
            var repository = _unitOfWork.Repository<SiteSetting>();
            var setting = await repository.GetAllAsQueryable()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (setting == null)
            {
                return Result<AdminSettingDto>.Success(new AdminSettingDto());
            }

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
