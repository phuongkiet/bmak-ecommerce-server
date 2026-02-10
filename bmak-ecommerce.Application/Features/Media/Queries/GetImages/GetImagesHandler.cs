using AutoMapper;
using AutoMapper.QueryableExtensions;
using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Media.DTOs;
using bmak_ecommerce.Domain.Entities.Media;
using bmak_ecommerce.Domain.Interfaces;
using bmak_ecommerce.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Media.Queries.GetImages
{
    [AutoRegister]

    public class GetImagesHandler : IQueryHandler<GetImagesQuery, PagedList<AppImageDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetImagesHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<AppImageDto>>> Handle(GetImagesQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Repository<AppImage>()
                .GetAllAsQueryable()
                .OrderByDescending(x => x.CreatedAt)
                .ProjectTo<AppImageDto>(_mapper.ConfigurationProvider);

            if (!string.IsNullOrEmpty(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(o => o.FileName.ToLower().Contains(search));
            }

            var count = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return Result<PagedList<AppImageDto>>.Success(new PagedList<AppImageDto>(items, count, request.PageIndex, request.PageSize));
        }
    }
}
