using System.Text;
using System.Text.RegularExpressions;

namespace bmak_ecommerce.Application.Features.RoomScenes.Commands.Common
{
    internal static class RoomSceneSlugHelper
    {
        public static string GenerateSlug(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            var lower = input.Trim().ToLowerInvariant();
            var normalized = lower.Normalize(NormalizationForm.FormD);
            var buffer = new StringBuilder(normalized.Length);

            foreach (var ch in normalized)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(ch);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    buffer.Append(ch);
                }
            }

            var noDiacritics = buffer.ToString().Normalize(NormalizationForm.FormC);
            var slug = Regex.Replace(noDiacritics, "[^a-z0-9\\s-]", string.Empty);
            slug = Regex.Replace(slug, "\\s+", "-");
            slug = Regex.Replace(slug, "-+", "-");

            return slug.Trim('-');
        }
    }
}
