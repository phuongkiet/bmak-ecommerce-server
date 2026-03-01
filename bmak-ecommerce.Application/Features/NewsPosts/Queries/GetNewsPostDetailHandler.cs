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
    public class GetNewsPostDetailHandler : IQueryHandler<GetNewsPostDetailQuery, NewsPostDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetNewsPostDetailHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<NewsPostDto>> Handle(GetNewsPostDetailQuery query, CancellationToken cancellationToken = default)
        {
            var post = await _unitOfWork.Repository<NewsPost>()
                .GetAllAsQueryable()
                .Include(x => x.Category)
                .Include(x => x.Author)
                .Where(x => x.Id == query.Id)
                .Select(x => new NewsPostDto
                {
                    Id = x.Id,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category.Name,
                    AuthorId = x.AuthorId,
                    AuthorName = x.Author != null ? x.Author.FullName : null,
                    Title = x.Title,
                    Slug = x.Slug,
                    Summary = x.Summary,
                    Content = x.Content,
                    ThumbnailUrl = x.ThumbnailUrl,
                    IsPublished = x.IsPublished,
                    ViewCount = x.ViewCount,
                    CreatedAt = x.CreatedAt,
                    PublishedAt = x.PublishedAt
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (post == null)
            {
                return Result<NewsPostDto>.Failure("Không tìm thấy bài viết tin tức.");
            }

            return Result<NewsPostDto>.Success(post);
        }
    }
}
