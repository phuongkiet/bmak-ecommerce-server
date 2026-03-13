using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Integrations.WordPress.Commands.SyncWordPressProduct;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace bmak_ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/webhooks/wordpress")]
    public class WordPressWebhookController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ICommandHandler<SyncWordPressProductCommand, SyncWordPressProductResult> _syncWordPressProductHandler;

        public WordPressWebhookController(
            IConfiguration configuration,
            ICommandHandler<SyncWordPressProductCommand, SyncWordPressProductResult> syncWordPressProductHandler)
        {
            _configuration = configuration;
            _syncWordPressProductHandler = syncWordPressProductHandler;
        }

        [AllowAnonymous]
        [HttpPost("product")]
        public async Task<ActionResult<ApiResponse<SyncWordPressProductResult>>> SyncProduct(
            [FromBody] WordPressProductWebhookRequest payload,
            [FromQuery] string? token,
            CancellationToken cancellationToken)
        {
            if (!IsTokenValid(token))
            {
                return Unauthorized(ApiResponse<SyncWordPressProductResult>.Failure("Invalid webhook token"));
            }

            var command = new SyncWordPressProductCommand
            {
                WordPressId = payload.Id,
                Name = payload.Name ?? payload.Title,
                Sku = payload.Sku,
                Slug = payload.GetSlugSource(),
                ShortDescription = payload.ShortDescription,
                Description = payload.Description,
                ThumbnailUrl = payload.GetPrimaryImageUrl(),
                BasePrice = ParseDecimal(payload.RegularPrice ?? payload.Price),
                SalePrice = ParseDecimal(payload.SalePrice ?? payload.Price),
                IsActive = ParseIsActive(payload.Status),
                Attributes = payload.GetMergedAttributes(),
                TierPrices = payload.GetTierPrices(),
                LevelDiscounts = payload.GetLevelDiscounts(),
                ReplaceLevelDiscounts = payload.RoleBasePrices != null,
                PriceUnit = payload.GetUnit(),
                SalesUnit = payload.GetUnit(),
                BoxQuantity = payload.GetItem(),
                Thickness = payload.GetThickness(),
                Random = payload.GetStripe(),
                ManageStock = ParseYesNo(payload.ManageStock),
                AllowBackorder = ParseYesNo(payload.Backorders)
            };

            var result = await _syncWordPressProductHandler.Handle(command, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse<SyncWordPressProductResult>.Failure(result.Error ?? "Sync failed"));
            }

            return Ok(ApiResponse<SyncWordPressProductResult>.Success(result.Value, "WordPress product synced"));
        }

        private bool IsTokenValid(string? queryToken)
        {
            var expectedToken = _configuration["WordPressWebhook:Token"];
            if (string.IsNullOrWhiteSpace(expectedToken))
            {
                return true;
            }

            var headerToken = Request.Headers["X-Webhook-Token"].FirstOrDefault();
            var incomingToken = !string.IsNullOrWhiteSpace(headerToken) ? headerToken : queryToken;

            return string.Equals(expectedToken, incomingToken, StringComparison.Ordinal);
        }

        private static decimal? ParseDecimal(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
            {
                return parsed;
            }

            return null;
        }

        private static bool? ParseIsActive(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return null;
            }

            return !status.Equals("draft", StringComparison.OrdinalIgnoreCase)
                && !status.Equals("trash", StringComparison.OrdinalIgnoreCase)
                && !status.Equals("private", StringComparison.OrdinalIgnoreCase);
        }

        private static bool? ParseYesNo(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            if (value.Equals("yes", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (value.Equals("no", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return null;
        }
    }

    public class WordPressProductWebhookRequest
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("sku")]
        public string? Sku { get; set; }

        [JsonPropertyName("slug")]
        public string? Slug { get; set; }

        [JsonPropertyName("short_description")]
        public string? ShortDescription { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("price")]
        public string? Price { get; set; }

        [JsonPropertyName("regular_price")]
        public string? RegularPrice { get; set; }

        [JsonPropertyName("sale_price")]
        public string? SalePrice { get; set; }

        [JsonPropertyName("manage_stock")]
        public string? ManageStock { get; set; }

        [JsonPropertyName("backorders")]
        public string? Backorders { get; set; }

        [JsonPropertyName("thumbnail_url")]
        public string? ThumbnailUrl { get; set; }

        [JsonPropertyName("image")]
        public WordPressImagePayload? Image { get; set; }

        [JsonPropertyName("featured_image")]
        public WordPressImagePayload? FeaturedImage { get; set; }

        [JsonPropertyName("images")]
        public List<WordPressImagePayload>? Images { get; set; }

        [JsonPropertyName("default_attributes")]
        public Dictionary<string, string>? DefaultAttributes { get; set; }

        [JsonPropertyName("default_attributes_serialized")]
        public string? DefaultAttributesSerialized { get; set; }

        [JsonPropertyName("extra_fields")]
        public WordPressExtraFieldsPayload? ExtraFields { get; set; }

        [JsonPropertyName("extra_fields_serialized")]
        public string? ExtraFieldsSerialized { get; set; }

        [JsonPropertyName("tier_prices")]
        public List<WordPressTierPricePayload>? TierPrices { get; set; }

        [JsonPropertyName("role_base_prices")]
        public Dictionary<string, WordPressRoleBasePricePayload>? RoleBasePrices { get; set; }

        [JsonPropertyName("weight")]
        public string? Weight { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement>? AdditionalData { get; set; }

        public string? GetPrimaryImageUrl()
        {
            if (!string.IsNullOrWhiteSpace(ThumbnailUrl))
            {
                return ThumbnailUrl;
            }

            if (!string.IsNullOrWhiteSpace(FeaturedImage?.Src))
            {
                return FeaturedImage.Src;
            }

            if (!string.IsNullOrWhiteSpace(FeaturedImage?.Url))
            {
                return FeaturedImage.Url;
            }

            if (!string.IsNullOrWhiteSpace(Image?.Src))
            {
                return Image.Src;
            }

            if (!string.IsNullOrWhiteSpace(Image?.Url))
            {
                return Image.Url;
            }

            var first = Images?.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(first?.Src))
            {
                return first.Src;
            }

            return first?.Url;
        }

        public Dictionary<string, string> GetMergedAttributes()
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (DefaultAttributes != null)
            {
                foreach (var item in DefaultAttributes)
                {
                    if (!string.IsNullOrWhiteSpace(item.Key) && !string.IsNullOrWhiteSpace(item.Value))
                    {
                        result[item.Key] = item.Value;
                    }
                }
            }

            foreach (var item in ParseSerializedPairs(DefaultAttributesSerialized))
            {
                result[item.Key] = item.Value;
            }

            if (AdditionalData != null)
            {
                foreach (var (key, value) in AdditionalData)
                {
                    if (!key.StartsWith("attribute_pa_", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var attrKey = key[10..];
                    var attrValue = value.ValueKind == JsonValueKind.String ? value.GetString() : value.GetRawText();
                    if (!string.IsNullOrWhiteSpace(attrValue))
                    {
                        result[attrKey] = attrValue;
                    }
                }
            }

            return result;
        }

        public List<SyncWordPressTierPriceInput> GetTierPrices()
        {
            var result = new List<SyncWordPressTierPriceInput>();

            if (TierPrices != null)
            {
                foreach (var tier in TierPrices)
                {
                    if (tier.TryConvert(out var converted))
                    {
                        result.Add(converted);
                    }
                }
            }

            return result;
        }

        public List<SyncWordPressLevelDiscountInput> GetLevelDiscounts()
        {
            var result = new List<SyncWordPressLevelDiscountInput>();
            if (RoleBasePrices == null)
            {
                return result;
            }

            foreach (var item in RoleBasePrices)
            {
                if (item.Value.TryConvertToLevelDiscount(item.Key, out var converted))
                {
                    result.Add(converted);
                }
            }

            return result;
        }

        public string? GetUnit()
        {
            if (!string.IsNullOrWhiteSpace(ExtraFields?.Unit))
            {
                return ExtraFields.Unit;
            }

            return GetExtraFieldString("unit");
        }

        public int? GetItem()
        {
            if (ExtraFields?.Item.HasValue == true)
            {
                return ExtraFields.Item;
            }

            if (ExtraFields?.BoxQuantity.HasValue == true)
            {
                return ExtraFields.BoxQuantity;
            }

            return GetExtraFieldInt("item", "box_quantity", "boxQuantity", "box_qty", "quantity_per_box");
        }

        public string? GetSlugSource()
        {
            if (!string.IsNullOrWhiteSpace(Title))
            {
                return Title;
            }

            if (!string.IsNullOrWhiteSpace(Name))
            {
                return Name;
            }

            return Slug;
        }

        public decimal? GetThickness()
        {
            if (ExtraFields?.Thick.HasValue == true)
            {
                return ExtraFields.Thick;
            }

            return GetExtraFieldDecimal("thick", "thickness");
        }

        public int? GetStripe()
        {
            if (ExtraFields?.Stripe.HasValue == true)
            {
                return ExtraFields.Stripe;
            }

            if (ExtraFields?.Random.HasValue == true)
            {
                return ExtraFields.Random;
            }

            return GetExtraFieldInt("stripe", "random");
        }

        private string? GetExtraFieldString(params string[] keys)
        {
            var pairs = ParseSerializedPairs(ExtraFieldsSerialized);

            foreach (var key in keys)
            {
                if (pairs.TryGetValue(key, out var raw) && !string.IsNullOrWhiteSpace(raw))
                {
                    return raw;
                }

                if (TryReadRootValue(key, out var rootValue))
                {
                    var rootRaw = ReadAsString(rootValue);
                    if (!string.IsNullOrWhiteSpace(rootRaw))
                    {
                        return rootRaw;
                    }
                }

                if (TryReadNestedExtraFieldsValue(key, out var extraValue))
                {
                    var extraRaw = ReadAsString(extraValue);
                    if (!string.IsNullOrWhiteSpace(extraRaw))
                    {
                        return extraRaw;
                    }
                }
            }

            return null;
        }

        private int? GetExtraFieldInt(params string[] keys)
        {
            var pairs = ParseSerializedPairs(ExtraFieldsSerialized);

            foreach (var key in keys)
            {
                if (pairs.TryGetValue(key, out var raw))
                {
                    var parsedFromSerialized = ParseNullableInt(raw);
                    if (parsedFromSerialized.HasValue)
                    {
                        return parsedFromSerialized;
                    }
                }

                if (TryReadRootValue(key, out var rootValue))
                {
                    var parsedFromRoot = ParseNullableInt(ReadAsString(rootValue));
                    if (parsedFromRoot.HasValue)
                    {
                        return parsedFromRoot;
                    }
                }

                if (TryReadNestedExtraFieldsValue(key, out var extraValue))
                {
                    var parsedFromNested = ParseNullableInt(ReadAsString(extraValue));
                    if (parsedFromNested.HasValue)
                    {
                        return parsedFromNested;
                    }
                }
            }

            return null;
        }

        private decimal? GetExtraFieldDecimal(params string[] keys)
        {
            var pairs = ParseSerializedPairs(ExtraFieldsSerialized);

            foreach (var key in keys)
            {
                if (pairs.TryGetValue(key, out var raw))
                {
                    var parsedFromSerialized = ParseNullableDecimal(raw);
                    if (parsedFromSerialized.HasValue)
                    {
                        return parsedFromSerialized;
                    }
                }

                if (TryReadRootValue(key, out var rootValue))
                {
                    var parsedFromRoot = ParseNullableDecimal(ReadAsString(rootValue));
                    if (parsedFromRoot.HasValue)
                    {
                        return parsedFromRoot;
                    }
                }

                if (TryReadNestedExtraFieldsValue(key, out var extraValue))
                {
                    var parsedFromNested = ParseNullableDecimal(ReadAsString(extraValue));
                    if (parsedFromNested.HasValue)
                    {
                        return parsedFromNested;
                    }
                }
            }

            return null;
        }

        private bool TryReadRootValue(string key, out JsonElement value)
        {
            value = default;
            if (AdditionalData == null)
            {
                return false;
            }

            if (AdditionalData.TryGetValue(key, out value))
            {
                return true;
            }

            var found = AdditionalData.FirstOrDefault(x =>
                string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(found.Key))
            {
                value = found.Value;
                return true;
            }

            return false;
        }

        private bool TryReadNestedExtraFieldsValue(string key, out JsonElement value)
        {
            value = default;

            if (ExtraFields?.AdditionalData != null)
            {
                if (TryGetCaseInsensitiveJsonProperty(ExtraFields.AdditionalData, key, out value))
                {
                    return true;
                }
            }

            if (AdditionalData == null
                || !TryGetCaseInsensitiveJsonProperty(AdditionalData, "extra_fields", out var extraFields)
                || extraFields.ValueKind != JsonValueKind.Object)
            {
                return false;
            }

            foreach (var property in extraFields.EnumerateObject())
            {
                if (string.Equals(property.Name, key, StringComparison.OrdinalIgnoreCase))
                {
                    value = property.Value;
                    return true;
                }
            }

            return false;
        }

        private static bool TryGetCaseInsensitiveJsonProperty(
            Dictionary<string, JsonElement> source,
            string key,
            out JsonElement value)
        {
            value = default;

            if (source.TryGetValue(key, out value))
            {
                return true;
            }

            var found = source.FirstOrDefault(x =>
                string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(found.Key))
            {
                value = found.Value;
                return true;
            }

            return false;
        }

        private static string? ReadAsString(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString(),
                JsonValueKind.Number => element.GetRawText(),
                JsonValueKind.True => "1",
                JsonValueKind.False => "0",
                _ => null
            };
        }

        private static int? ParseNullableInt(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return null;
            }

            if (int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var asInt))
            {
                return asInt;
            }

            if (decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var asDecimal))
            {
                return (int)decimal.Round(asDecimal, 0, MidpointRounding.AwayFromZero);
            }

            return null;
        }

        private static decimal? ParseNullableDecimal(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return null;
            }

            if (decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
            {
                return value;
            }

            return null;
        }

        private static Dictionary<string, string> ParseSerializedPairs(string? serialized)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(serialized))
            {
                return result;
            }

            // Parse simple php serialized key/value pairs: s:n:"key";s:n:"value";
            var pattern = @"s:\d+:\""(?<key>[^\""]+)\"";s:\d+:\""(?<value>[^\""]*)\"";";
            foreach (Match match in Regex.Matches(serialized, pattern))
            {
                var key = match.Groups["key"].Value;
                var value = match.Groups["value"].Value;
                if (!string.IsNullOrWhiteSpace(key))
                {
                    result[key] = value;
                }
            }

            return result;
        }
    }

    public class WordPressImagePayload
    {
        [JsonPropertyName("src")]
        public string? Src { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }

    public class WordPressExtraFieldsPayload
    {
        [JsonPropertyName("unit")]
        public string? Unit { get; set; }

        [JsonPropertyName("item")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int? Item { get; set; }

        [JsonPropertyName("box_quantity")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int? BoxQuantity { get; set; }

        [JsonPropertyName("thick")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal? Thick { get; set; }

        [JsonPropertyName("stripe")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int? Stripe { get; set; }

        [JsonPropertyName("random")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int? Random { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement>? AdditionalData { get; set; }
    }

    public class WordPressTierPricePayload
    {
        [JsonPropertyName("min_quantity")]
        public float? MinQuantity { get; set; }

        [JsonPropertyName("price")]
        public decimal? Price { get; set; }

        [JsonPropertyName("discount_price")]
        public decimal? DiscountPrice { get; set; }

        public bool TryConvert(out SyncWordPressTierPriceInput value)
        {
            var resolvedPrice = DiscountPrice ?? Price;
            if (!resolvedPrice.HasValue)
            {
                value = new SyncWordPressTierPriceInput();
                return false;
            }

            value = new SyncWordPressTierPriceInput
            {
                MinQuantity = MinQuantity ?? 1,
                Price = resolvedPrice.Value
            };

            return true;
        }
    }

    public class WordPressRoleBasePricePayload
    {
        [JsonPropertyName("price")]
        public decimal? Price { get; set; }

        [JsonPropertyName("discount_price")]
        public decimal? DiscountPrice { get; set; }

        [JsonPropertyName("discount_type")]
        public string? DiscountType { get; set; }

        [JsonPropertyName("discount_value")]
        public decimal? DiscountValue { get; set; }

        [JsonPropertyName("discount_percent")]
        public decimal? DiscountPercent { get; set; }

        public bool TryConvertToLevelDiscount(string levelKey, out SyncWordPressLevelDiscountInput value)
        {
            value = new SyncWordPressLevelDiscountInput();

            var match = Regex.Match(levelKey ?? string.Empty, @"level_(\d+)", RegexOptions.IgnoreCase);
            if (!match.Success || !int.TryParse(match.Groups[1].Value, out var rank) || rank <= 0)
            {
                return false;
            }

            decimal? percent = DiscountPercent;

            if (!percent.HasValue && DiscountValue.HasValue)
            {
                if (string.Equals(DiscountType, "percentage_decrease", StringComparison.OrdinalIgnoreCase))
                {
                    percent = DiscountValue.Value;
                }
                else if (string.Equals(DiscountType, "price_decrease", StringComparison.OrdinalIgnoreCase)
                    && Price.HasValue && Price.Value > 0)
                {
                    percent = (DiscountValue.Value / Price.Value) * 100m;
                }
            }

            if (!percent.HasValue && Price.HasValue && DiscountPrice.HasValue && Price.Value > 0)
            {
                percent = ((Price.Value - DiscountPrice.Value) / Price.Value) * 100m;
            }

            if (!percent.HasValue)
            {
                return false;
            }

            var normalized = Math.Clamp(percent.Value, 0m, 100m);
            value = new SyncWordPressLevelDiscountInput
            {
                LevelRank = rank,
                DiscountPercent = decimal.Round(normalized, 2, MidpointRounding.AwayFromZero),
                IsActive = normalized > 0
            };

            return true;
        }
    }
}
