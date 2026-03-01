using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.NewsPosts.DTOs;
using bmak_ecommerce.Domain.Entities.NewFolder;
using bmak_ecommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bmak_ecommerce.Application.Features.NewsPosts.Queries
{
    [AutoRegister]
    public class GetNewsPostsHandler : IQueryHandler<GetNewsPostsQuery, List<NewsPostSummaryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetNewsPostsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<NewsPostSummaryDto>>> Handle(GetNewsPostsQuery query, CancellationToken cancellationToken = default)
        {
            var posts = await _unitOfWork.Repository<NewsPost>()
                .GetAllAsQueryable()
                .Include(x => x.Category)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new NewsPostSummaryDto
                {
                    Id = x.Id,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category.Name,
                    Title = x.Title,
                    Slug = x.Slug,
                    Summary = x.Summary,
                    ThumbnailUrl = x.ThumbnailUrl,
                    IsPublished = x.IsPublished,
                    ViewCount = x.ViewCount,
                    CreatedAt = x.CreatedAt,
                    PublishedAt = x.PublishedAt
                })
                .ToListAsync(cancellationToken);

            return Result<List<NewsPostSummaryDto>>.Success(posts);
        }
    }
}
