using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Entities.NewFolder;
using bmak_ecommerce.Domain.Interfaces;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace bmak_ecommerce.Application.Features.NewsCategories.Commands.UpdateNewsCategory
{
    [AutoRegister]
    public class UpdateNewsCategoryHandler : ICommandHandler<UpdateNewsCategoryCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateNewsCategoryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateNewsCategoryCommand command, CancellationToken cancellationToken = default)
        {
            var category = await _unitOfWork.Repository<NewsCategory>().GetByIdAsync(command.Id);
            if (category == null)
            {
                return Result<bool>.Failure("Không tìm thấy danh mục tin tức.");
            }

            var newSlug = GenerateSlug(command.Name);
            if (!string.Equals(category.Slug, newSlug, StringComparison.OrdinalIgnoreCase))
            {
                var existing = await _unitOfWork.Repository<NewsCategory>().FindAsync(x => x.Slug == newSlug);
                if (existing.Any(x => x.Id != command.Id))
                {
                    return Result<bool>.Failure($"Danh mục với slug '{newSlug}' đã tồn tại.");
                }
            }

            category.Name = command.Name;
            category.Description = command.Description;
            category.Slug = newSlug;

            _unitOfWork.Repository<NewsCategory>().Update(category);
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
