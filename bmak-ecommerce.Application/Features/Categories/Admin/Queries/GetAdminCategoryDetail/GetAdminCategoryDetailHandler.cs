using AutoMapper;
using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using bmak_ecommerce.Domain.Interfaces;

namespace bmak_ecommerce.Application.Features.Categories.Admin.Queries.GetAdminCategoryDetail
{
    [AutoRegister]
    public class GetAdminCategoryDetailHandler : IQueryHandler<GetAdminCategoryDetailQuery, CategoryDto>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public GetAdminCategoryDetailHandler(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<Result<CategoryDto>> Handle(GetAdminCategoryDetailQuery query, CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetCategoryDetailAsync(query.Id);
            if (category == null)
            {
                return Result<CategoryDto>.Failure("Khong tim thay danh muc.");
            }

            var dto = _mapper.Map<CategoryDto>(category);
            return Result<CategoryDto>.Success(dto);
        }
    }
}
