using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.NewsCategories.DTOs;
using bmak_ecommerce.Domain.Entities.NewFolder;
using bmak_ecommerce.Domain.Interfaces;

namespace bmak_ecommerce.Application.Features.NewsCategories.Queries
{
    [AutoRegister]
    public class GetNewsCategoriesHandler : IQueryHandler<GetNewsCategoriesQuery, List<NewsCategoryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetNewsCategoriesHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<NewsCategoryDto>>> Handle(GetNewsCategoriesQuery query, CancellationToken cancellationToken = default)
        {
            var categories = await _unitOfWork.Repository<NewsCategory>().GetAllAsync();

            var result = categories
                .OrderBy(x => x.Name)
                .Select(x => new NewsCategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Slug = x.Slug,
                    Description = x.Description
                })
                .ToList();

            return Result<List<NewsCategoryDto>>.Success(result);
        }
    }
}
