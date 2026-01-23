using AutoMapper;
using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using bmak_ecommerce.Domain.Interfaces;
using bmak_ecommerce.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Categories.Queries
{
    public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, PagedList<CategoryDto>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public GetCategoriesHandler(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<PagedList<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            // 1. Gọi Repository để lấy PagedList<Entity>
            var pagedEntities = await _categoryRepository.GetCategoriesAsync(request.Params);

            // 2. Map sang PagedList<DTO>
            // Lưu ý: Chỉ map phần Items, các thông số phân trang giữ nguyên
            var dtos = _mapper.Map<List<CategoryDto>>(pagedEntities.Items);

            return new PagedList<CategoryDto>(
                dtos,
                pagedEntities.TotalCount,
                pagedEntities.PageIndex,
                request.Params.PageSize
            );
        }
    }
}

