using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Provinces.Dtos;
using bmak_ecommerce.Application.Features.Provinces.Queries;
using bmak_ecommerce.Application.Features.Wards.Dtos;
using bmak_ecommerce.Domain.Interfaces;
using bmak_ecommerce.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Wards.Queries
{
    [AutoRegister]

    public class GetWardHandler : IQueryHandler<GetWardQuery, PagedList<WardDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetWardHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagedList<WardDto>>> Handle(GetWardQuery request, CancellationToken cancellationToken = default)
        {
            var specParams = request.Params;

            // Get paginated provinces from repository
            var pagedWards = await _unitOfWork.Wards.GetWardsByProvinceAsync(specParams);

            // Map to DTO
            var dtos = pagedWards.Items.Select(p => new WardDto
            {
                Id = p.Id,
                Name = p.Name
            }).ToList();

            // Create PagedList with DTOs
            var pagedListDto = new PagedList<WardDto>(
                dtos,
                pagedWards.TotalCount,
                pagedWards.PageIndex,
                pagedWards.PageSize
            );

            return Result<PagedList<WardDto>>.Success(pagedListDto);
        }
    }
}
