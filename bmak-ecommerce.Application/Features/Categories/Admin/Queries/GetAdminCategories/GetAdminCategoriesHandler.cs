using AutoMapper;
using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using bmak_ecommerce.Domain.Interfaces;
using bmak_ecommerce.Domain.Models;

namespace bmak_ecommerce.Application.Features.Categories.Admin.Queries.GetAdminCategories
{
    [AutoRegister]
    public class GetAdminCategoriesHandler : IQueryHandler<GetAdminCategoriesQuery, PagedList<CategoryDto>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public GetAdminCategoriesHandler(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<CategoryDto>>> Handle(GetAdminCategoriesQuery query, CancellationToken cancellationToken = default)
        {
            var pagedEntities = await _categoryRepository.GetCategoriesAsync(query.Params);
            var dtos = _mapper.Map<List<CategoryDto>>(pagedEntities.Items);

            var result = new PagedList<CategoryDto>(
                dtos,
                pagedEntities.TotalCount,
                pagedEntities.PageIndex,
                query.Params.PageSize
            );

            return Result<PagedList<CategoryDto>>.Success(result);
        }
    }
}
