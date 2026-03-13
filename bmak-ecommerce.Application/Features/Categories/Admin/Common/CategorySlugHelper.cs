using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace bmak_ecommerce.Application.Features.Categories.Admin.Common
{
    internal static class CategorySlugHelper
    {
        public static string GenerateSlug(string phrase)
        {
            if (string.IsNullOrWhiteSpace(phrase)) return string.Empty;

            var str = phrase.ToLower().Trim().Replace("đ", "d");
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
