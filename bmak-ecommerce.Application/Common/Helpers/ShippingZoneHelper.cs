using System.Globalization;
using System.Text;

namespace bmak_ecommerce.Application.Common.Helpers
{
    public static class ShippingZoneHelper
    {
        public const string UnknownZone = "unknown";
        public const string HcmCoreZone = "hcm_core";
        public const string HanoiCoreZone = "hanoi_core";
        public const string NorthZone = "north";
        public const string CentralZone = "central";
        public const string SouthZone = "south";

        private static readonly HashSet<string> NorthProvinceIds = new(StringComparer.OrdinalIgnoreCase)
        {
            "04", "08", "11", "12", "14", "15", "19", "20", "22", "24", "25", "31", "33", "37"
        };

        private static readonly HashSet<string> CentralProvinceIds = new(StringComparer.OrdinalIgnoreCase)
        {
            "38", "40", "42", "44", "46", "48", "51", "52", "56", "66", "68"
        };

        private static readonly HashSet<string> SouthProvinceIds = new(StringComparer.OrdinalIgnoreCase)
        {
            "75", "80", "82", "86", "91", "92", "96"
        };

        private static readonly HashSet<string> NorthProvinceNames = new(StringComparer.OrdinalIgnoreCase)
        {
            "cao bang", "tuyen quang", "dien bien", "lai chau", "son la", "lao cai", "thai nguyen",
            "lang son", "quang ninh", "bac ninh", "phu tho", "hai phong", "hung yen", "ninh binh"
        };

        private static readonly HashSet<string> CentralProvinceNames = new(StringComparer.OrdinalIgnoreCase)
        {
            "thanh hoa", "nghe an", "ha tinh", "quang tri", "hue", "da nang", "quang ngai",
            "gia lai", "khanh hoa", "dak lak", "lam dong"
        };

        private static readonly HashSet<string> SouthProvinceNames = new(StringComparer.OrdinalIgnoreCase)
        {
            "dong nai", "tay ninh", "dong thap", "vinh long", "an giang", "can tho", "ca mau"
        };

        public static string Resolve(string provinceValue, string? provinceName = null)
        {
            var normalizedInput = Normalize(provinceValue);
            if (TryResolveByProvinceId(normalizedInput, out var zone))
            {
                return zone;
            }

            if (TryResolveByProvinceName(normalizedInput, out zone))
            {
                return zone;
            }

            var normalizedName = Normalize(provinceName);
            if (TryResolveByProvinceName(normalizedName, out zone))
            {
                return zone;
            }

            return UnknownZone;
        }

        private static bool TryResolveByProvinceId(string provinceId, out string zone)
        {
            if (string.Equals(provinceId, "79", StringComparison.OrdinalIgnoreCase))
            {
                zone = HcmCoreZone;
                return true;
            }

            if (string.Equals(provinceId, "01", StringComparison.OrdinalIgnoreCase))
            {
                zone = HanoiCoreZone;
                return true;
            }

            if (NorthProvinceIds.Contains(provinceId))
            {
                zone = NorthZone;
                return true;
            }

            if (CentralProvinceIds.Contains(provinceId))
            {
                zone = CentralZone;
                return true;
            }

            if (SouthProvinceIds.Contains(provinceId))
            {
                zone = SouthZone;
                return true;
            }

            zone = UnknownZone;
            return false;
        }

        private static bool TryResolveByProvinceName(string provinceName, out string zone)
        {
            if (string.IsNullOrWhiteSpace(provinceName))
            {
                zone = UnknownZone;
                return false;
            }

            if (provinceName.Contains("ho chi minh", StringComparison.OrdinalIgnoreCase))
            {
                zone = HcmCoreZone;
                return true;
            }

            if (provinceName.Contains("ha noi", StringComparison.OrdinalIgnoreCase))
            {
                zone = HanoiCoreZone;
                return true;
            }

            if (NorthProvinceNames.Contains(provinceName))
            {
                zone = NorthZone;
                return true;
            }

            if (CentralProvinceNames.Contains(provinceName))
            {
                zone = CentralZone;
                return true;
            }

            if (SouthProvinceNames.Contains(provinceName))
            {
                zone = SouthZone;
                return true;
            }

            zone = UnknownZone;
            return false;
        }

        private static string Normalize(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var withoutDiacritics = RemoveDiacritics(value.Trim()).ToLowerInvariant();

            if (withoutDiacritics.StartsWith("tinh "))
            {
                withoutDiacritics = withoutDiacritics[5..];
            }

            if (withoutDiacritics.StartsWith("thanh pho "))
            {
                withoutDiacritics = withoutDiacritics[10..];
            }

            return withoutDiacritics.Trim();
        }

        private static string RemoveDiacritics(string value)
        {
            var normalized = value.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder(normalized.Length);

            foreach (var c in normalized)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(c);
                if (category != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(c);
                }
            }

            return builder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}