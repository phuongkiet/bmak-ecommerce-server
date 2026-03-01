using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.NewsCategories.DTOs;
using bmak_ecommerce.Domain.Entities.NewFolder;
using bmak_ecommerce.Domain.Interfaces;

namespace bmak_ecommerce.Application.Features.NewsCategories.Queries
{
    [AutoRegister]
    public class GetNewsCategoryDetailHandler : IQueryHandler<GetNewsCategoryDetailQuery, NewsCategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetNewsCategoryDetailHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<NewsCategoryDto>> Handle(GetNewsCategoryDetailQuery query, CancellationToken cancellationToken = default)
        {
            var category = await _unitOfWork.Repository<NewsCategory>().GetByIdAsync(query.Id);

            if (category == null)
            {
                return Result<NewsCategoryDto>.Failure("Không tìm thấy danh mục tin tức.");
            }

            var dto = new NewsCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                Description = category.Description
            };

            return Result<NewsCategoryDto>.Success(dto);
        }
    }
}
