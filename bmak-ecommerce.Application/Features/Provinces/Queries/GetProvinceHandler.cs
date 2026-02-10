using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Provinces.Dtos;
using bmak_ecommerce.Domain.Interfaces;
using bmak_ecommerce.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace bmak_ecommerce.Application.Features.Provinces.Queries
{
    [AutoRegister]

    public class GetProvinceHandler : IQueryHandler<GetProvinceQuery, PagedList<ProvinceDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetProvinceHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagedList<ProvinceDto>>> Handle(GetProvinceQuery request, CancellationToken cancellationToken = default)
        {
            var specParams = request.Params;

            // Get paginated provinces from repository
            var pagedProvinces = await _unitOfWork.Provinces.GetProvincesAsync(specParams);

            // Map to DTO
            var dtos = pagedProvinces.Items.Select(p => new ProvinceDto
            {
                Id = p.Id,
                Name = p.Name
            }).ToList();

            // Create PagedList with DTOs
            var pagedListDto = new PagedList<ProvinceDto>(
                dtos,
                pagedProvinces.TotalCount,
                pagedProvinces.PageIndex,
                pagedProvinces.PageSize
            );

            return Result<PagedList<ProvinceDto>>.Success(pagedListDto);
        }
    }
}
