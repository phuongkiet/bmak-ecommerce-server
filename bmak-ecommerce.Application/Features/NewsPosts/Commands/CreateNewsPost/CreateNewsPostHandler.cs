using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Entities.Identity;
using bmak_ecommerce.Domain.Entities.NewFolder;
using bmak_ecommerce.Domain.Interfaces;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace bmak_ecommerce.Application.Features.NewsPosts.Commands.CreateNewsPost
{
    [AutoRegister]
    public class CreateNewsPostHandler : ICommandHandler<CreateNewsPostCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateNewsPostHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(CreateNewsPostCommand command, CancellationToken cancellationToken = default)
        {
            var category = await _unitOfWork.Repository<NewsCategory>().GetByIdAsync(command.CategoryId);
            if (category == null)
            {
                return Result<int>.Failure("Danh mục tin tức không tồn tại.");
            }

            if (command.AuthorId.HasValue)
            {
                var author = await _unitOfWork.Repository<AppUser>().GetByIdAsync(command.AuthorId.Value);
                if (author == null)
                {
                    return Result<int>.Failure("Tác giả không tồn tại.");
                }
            }

            var slug = GenerateSlug(command.Title);
            var existing = await _unitOfWork.Repository<NewsPost>().FindAsync(x => x.Slug == slug);
            if (existing.Any())
            {
                return Result<int>.Failure($"Bài viết với slug '{slug}' đã tồn tại.");
            }

            var post = new NewsPost
            {
                CategoryId = command.CategoryId,
                AuthorId = command.AuthorId,
                Title = command.Title,
                Slug = slug,
                Summary = command.Summary,
                Content = command.Content,
                ThumbnailUrl = command.ThumbnailUrl,
                IsPublished = command.IsPublished,
                PublishedAt = command.IsPublished ? DateTime.UtcNow : null,
                ViewCount = 0,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<NewsPost>().AddAsync(post);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(post.Id);
        }

        private static string GenerateSlug(string phrase)
        {
            if (string.IsNullOrWhiteSpace(phrase)) return string.Empty;

            string str = phrase.ToLower().Trim().Replace("đ", "d");
            var normalized = str.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder(capacity: normalized.Length);

            foreach (var c in normalized)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(c);
                }
            }

            str = builder.ToString().Normalize(NormalizationForm.FormC);
            str = Regex.Replace(str, @"[^a-z0-9\s-]", string.Empty);
            str = Regex.Replace(str, @"\s+", "-").Trim('-');
            str = Regex.Replace(str, @"-+", "-");

            return str;
        }
    }
}
