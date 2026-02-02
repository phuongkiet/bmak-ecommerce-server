using AutoMapper;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Pages.DTOs;
using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using bmak_ecommerce.Application.Features.Products.Queries.Products.GetProductById;
using bmak_ecommerce.Domain.Entities.Page;
using bmak_ecommerce.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Pages.Queries.GetPageDetail
{
    public class GetPageDetailHandler : IQueryHandler<GetPageDetailQuery, PageDto>
    {
        private readonly IUnitOfWork _unitOfWork; // Dùng UoW để access repository
        private readonly IMapper _mapper;

        public GetPageDetailHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<PageDto>> Handle(GetPageDetailQuery request, CancellationToken cancellationToken)
        {
            Page page = null;

            // Ưu tiên tìm theo Slug
            if (!string.IsNullOrEmpty(request.Slug))
            {
                page = await _unitOfWork.Pages.GetBySlugAsync(request.Slug);
            }
            else if (request.Id.HasValue)
            {
                page = await _unitOfWork.Repository<Page>().GetByIdAsync(request.Id.Value);
            }

            if (page == null) return Result<PageDto>.Failure("Không tìm thấy trang nội dung.");

            var pageDto = new PageDto
            {
                Id = page.Id,
                Title = page.Title,
                Slug = page.Slug,
                // DESERIALIZE: String -> List Object
                Sections = string.IsNullOrEmpty(page.ContentJson)
                    ? new List<PageSectionDto>()
                    : JsonSerializer.Deserialize<List<PageSectionDto>>(page.ContentJson, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
            };

            return Result<PageDto>.Success(pageDto);
        }
    }
}
