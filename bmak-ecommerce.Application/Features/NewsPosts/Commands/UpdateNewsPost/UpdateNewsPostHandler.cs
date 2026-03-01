using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Entities.Identity;
using bmak_ecommerce.Domain.Entities.NewFolder;
using bmak_ecommerce.Domain.Interfaces;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace bmak_ecommerce.Application.Features.NewsPosts.Commands.UpdateNewsPost
{
    [AutoRegister]
    public class UpdateNewsPostHandler : ICommandHandler<UpdateNewsPostCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateNewsPostHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateNewsPostCommand command, CancellationToken cancellationToken = default)
        {
            var post = await _unitOfWork.Repository<NewsPost>().GetByIdAsync(command.Id);
            if (post == null)
            {
                return Result<bool>.Failure("Không tìm thấy bài viết tin tức.");
            }

            var category = await _unitOfWork.Repository<NewsCategory>().GetByIdAsync(command.CategoryId);
            if (category == null)
            {
                return Result<bool>.Failure("Danh mục tin tức không tồn tại.");
            }

            if (command.AuthorId.HasValue)
            {
                var author = await _unitOfWork.Repository<AppUser>().GetByIdAsync(command.AuthorId.Value);
                if (author == null)
                {
                    return Result<bool>.Failure("Tác giả không tồn tại.");
                }
            }

            var newSlug = GenerateSlug(command.Title);
            if (!string.Equals(post.Slug, newSlug, StringComparison.OrdinalIgnoreCase))
            {
                var existing = await _unitOfWork.Repository<NewsPost>().FindAsync(x => x.Slug == newSlug);
                if (existing.Any(x => x.Id != command.Id))
                {
                    return Result<bool>.Failure($"Bài viết với slug '{newSlug}' đã tồn tại.");
                }
            }

            var wasPublished = post.IsPublished;

            post.CategoryId = command.CategoryId;
            post.AuthorId = command.AuthorId;
            post.Title = command.Title;
            post.Slug = newSlug;
            post.Summary = command.Summary;
            post.Content = command.Content;
            post.ThumbnailUrl = command.ThumbnailUrl;
            post.IsPublished = command.IsPublished;

            if (!wasPublished && command.IsPublished)
            {
                post.PublishedAt = DateTime.UtcNow;
            }
            else if (!command.IsPublished)
            {
                post.PublishedAt = null;
            }

            _unitOfWork.Repository<NewsPost>().Update(post);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
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
