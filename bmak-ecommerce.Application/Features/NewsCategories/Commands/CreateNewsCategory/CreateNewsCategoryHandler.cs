using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Entities.NewFolder;
using bmak_ecommerce.Domain.Interfaces;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace bmak_ecommerce.Application.Features.NewsCategories.Commands.CreateNewsCategory
{
    [AutoRegister]
    public class CreateNewsCategoryHandler : ICommandHandler<CreateNewsCategoryCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateNewsCategoryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(CreateNewsCategoryCommand command, CancellationToken cancellationToken = default)
        {
            var slug = GenerateSlug(command.Name);
            var existing = await _unitOfWork.Repository<NewsCategory>().FindAsync(x => x.Slug == slug);

            if (existing.Any())
            {
                return Result<int>.Failure($"Danh mục với slug '{slug}' đã tồn tại.");
            }

            var category = new NewsCategory
            {
                Name = command.Name,
                Slug = slug,
                Description = command.Description
            };

            await _unitOfWork.Repository<NewsCategory>().AddAsync(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(category.Id);
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
