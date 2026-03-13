using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Entities.Identity;
using bmak_ecommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace bmak_ecommerce.Application.Features.Integrations.WordPress.Commands.SyncWordPressProduct
{
    [AutoRegister]
    public class SyncWordPressProductHandler : ICommandHandler<SyncWordPressProductCommand, SyncWordPressProductResult>
    {
        private const string DefaultSalesUnit = "m2";
        private const string DefaultPriceUnit = "cai";
        private const bool WordPressSoftStockManageStock = false;
        private const bool WordPressSoftStockAllowBackorder = true;

        private readonly IUnitOfWork _unitOfWork;

        public SyncWordPressProductHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<SyncWordPressProductResult>> Handle(SyncWordPressProductCommand request, CancellationToken cancellationToken = default)
        {
            var wordPressProductId = request.WordPressId;
            var normalizedSku = Normalize(request.Sku);
            var normalizedSlug = NormalizeSlug(request.Slug);
            var wordPressFallbackSku = wordPressProductId.HasValue
                ? $"WP-{wordPressProductId.Value}"
                : null;

            var productQuery = _unitOfWork.Repository<Product>()
                .GetAllAsQueryable()
                .Include(x => x.AttributeSelections)
                .Include(x => x.TierPrices)
                .Include(x => x.LevelDiscounts);

            Product? product = null;

            if (wordPressProductId.HasValue)
            {
                product = await productQuery.FirstOrDefaultAsync(
                    x => x.WordPressProductId == wordPressProductId.Value,
                    cancellationToken);
            }

            if (product == null && !string.IsNullOrWhiteSpace(wordPressFallbackSku))
            {
                product = await productQuery.FirstOrDefaultAsync(x => x.SKU == wordPressFallbackSku, cancellationToken);
            }

            if (product == null && !wordPressProductId.HasValue && !string.IsNullOrWhiteSpace(normalizedSku))
            {
                product = await productQuery.FirstOrDefaultAsync(x => x.SKU == normalizedSku, cancellationToken);
            }

            if (product == null && !wordPressProductId.HasValue && !string.IsNullOrWhiteSpace(normalizedSlug))
            {
                product = await productQuery.FirstOrDefaultAsync(x => x.Slug == normalizedSlug, cancellationToken);
            }

            if (product == null && !CanCreateFromPayload(request))
            {
                return Result<SyncWordPressProductResult>.Success(new SyncWordPressProductResult
                {
                    ProductId = 0,
                    Action = "skipped_incomplete"
                });
            }

            var isCreated = false;

            if (product == null)
            {
                var fallbackSku = await BuildSkuForInsertAsync(normalizedSku, wordPressProductId, cancellationToken);
                var fallbackName = Normalize(request.Name) ?? fallbackSku;
                var fallbackSlug = normalizedSlug ?? GenerateSlug(fallbackName);
                var basePrice = request.BasePrice ?? request.SalePrice ?? 0;
                var salePrice = request.SalePrice ?? request.BasePrice ?? 0;

                product = new Product
                {
                    Name = fallbackName,
                    SKU = fallbackSku,
                    Slug = fallbackSlug,
                    ShortDescription = request.ShortDescription,
                    Description = request.Description,
                    BasePrice = basePrice,
                    SalePrice = salePrice,
                    SalesUnit = Normalize(request.SalesUnit) ?? DefaultSalesUnit,
                    PriceUnit = Normalize(request.PriceUnit) ?? DefaultPriceUnit,
                    ConversionFactor = 1,
                    Weight = request.Weight ?? 0,
                    SpecificationsJson = "[]",
                    Thumbnail = Normalize(request.ThumbnailUrl),
                    IsActive = request.IsActive ?? true,
                    AllowBackorder = WordPressSoftStockAllowBackorder,
                    ManageStock = WordPressSoftStockManageStock,
                    Thickness = request.Thickness,
                    BoxQuantity = request.BoxQuantity,
                    Random = request.Random,
                    WordPressProductId = wordPressProductId
                };

                await _unitOfWork.Repository<Product>().AddAsync(product);
                isCreated = true;
            }
            else
            {
                if (wordPressProductId.HasValue)
                {
                    product.WordPressProductId = wordPressProductId.Value;
                }

                var requestName = Normalize(request.Name);
                if (!string.IsNullOrWhiteSpace(requestName))
                {
                    product.Name = requestName;
                }

                if (!string.IsNullOrWhiteSpace(normalizedSlug))
                {
                    product.Slug = normalizedSlug;
                }

                if (!string.IsNullOrWhiteSpace(normalizedSku))
                {
                    var isSkuTaken = await _unitOfWork.Repository<Product>()
                        .GetAllAsQueryable()
                        .AnyAsync(x => x.SKU == normalizedSku && x.Id != product.Id, cancellationToken);

                    if (isSkuTaken && !wordPressProductId.HasValue)
                    {
                        return Result<SyncWordPressProductResult>.Failure($"SKU '{normalizedSku}' da ton tai.");
                    }

                    if (!isSkuTaken)
                    {
                        product.SKU = normalizedSku;
                    }
                }

                if (request.ShortDescription != null)
                {
                    product.ShortDescription = request.ShortDescription;
                }

                if (request.Description != null)
                {
                    product.Description = request.Description;
                }

                if (request.BasePrice.HasValue)
                {
                    product.BasePrice = request.BasePrice.Value;
                }

                if (request.SalePrice.HasValue)
                {
                    product.SalePrice = request.SalePrice.Value;
                }

                var thumbnailUrl = Normalize(request.ThumbnailUrl);
                if (!string.IsNullOrWhiteSpace(thumbnailUrl))
                {
                    product.Thumbnail = thumbnailUrl;
                }

                if (request.IsActive.HasValue)
                {
                    product.IsActive = request.IsActive.Value;
                }

                if (!string.IsNullOrWhiteSpace(request.SalesUnit))
                {
                    product.SalesUnit = request.SalesUnit;
                }

                if (!string.IsNullOrWhiteSpace(request.PriceUnit))
                {
                    product.PriceUnit = request.PriceUnit;
                }

                if (request.Weight.HasValue)
                {
                    product.Weight = request.Weight.Value;
                }

                product.ManageStock = WordPressSoftStockManageStock;
                product.AllowBackorder = WordPressSoftStockAllowBackorder;

                if (request.Thickness.HasValue)
                {
                    product.Thickness = request.Thickness.Value;
                }

                if (request.BoxQuantity.HasValue)
                {
                    product.BoxQuantity = request.BoxQuantity.Value;
                }

                if (request.Random.HasValue)
                {
                    product.Random = request.Random.Value;
                }

                product.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Repository<Product>().Update(product);
            }

            if (request.Attributes.Count > 0)
            {
                await UpsertProductAttributesAsync(product, request.Attributes, cancellationToken);
            }

            if (request.TierPrices.Count > 0)
            {
                await UpsertTierPricesAsync(product, request.TierPrices);
            }

            if (request.ReplaceLevelDiscounts)
            {
                await UpsertLevelDiscountsAsync(product, request.LevelDiscounts, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<SyncWordPressProductResult>.Success(new SyncWordPressProductResult
            {
                ProductId = product.Id,
                Action = isCreated ? "created" : "updated",
                Sku = product.SKU,
                ThumbnailUrl = product.Thumbnail
            });
        }

        private async Task<string> BuildSkuForInsertAsync(string? incomingSku, int? wordPressId, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(incomingSku))
            {
                var incomingExists = await _unitOfWork.Repository<Product>()
                    .GetAllAsQueryable()
                    .AnyAsync(x => x.SKU == incomingSku, cancellationToken);

                if (!incomingExists)
                {
                    return incomingSku;
                }
            }

            var baseSku = wordPressId.HasValue
                ? $"WP-{wordPressId.Value}"
                : $"WP-{DateTime.UtcNow:yyyyMMddHHmmss}";

            var sku = baseSku;
            var suffix = 1;

            while (await _unitOfWork.Repository<Product>()
                .GetAllAsQueryable()
                .AnyAsync(x => x.SKU == sku, cancellationToken))
            {
                sku = $"{baseSku}-{suffix}";
                suffix++;
            }

            return sku;
        }

        private static string? Normalize(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static bool CanCreateFromPayload(SyncWordPressProductCommand request)
        {
            var hasName = !string.IsNullOrWhiteSpace(request.Name);
            var hasSku = !string.IsNullOrWhiteSpace(request.Sku);
            var hasPrice = request.BasePrice.HasValue || request.SalePrice.HasValue;
            var hasImage = !string.IsNullOrWhiteSpace(request.ThumbnailUrl);

            return hasName && (hasSku || hasPrice || hasImage);
        }

        private static string? NormalizeSlug(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return GenerateSlug(value);
        }

        private static string GenerateSlug(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return "san-pham";
            }

            var normalized = input.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(normalized.Length);
            var lastWasDash = false;

            foreach (var ch in normalized)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (category == UnicodeCategory.NonSpacingMark)
                {
                    continue;
                }

                char mapped = ch;
                if (mapped == 'đ')
                {
                    mapped = 'd';
                }

                if ((mapped >= 'a' && mapped <= 'z') || char.IsDigit(mapped))
                {
                    sb.Append(mapped);
                    lastWasDash = false;
                    continue;
                }

                if (!lastWasDash)
                {
                    sb.Append('-');
                    lastWasDash = true;
                }
            }

            var slug = sb.ToString().Trim('-');
            return string.IsNullOrWhiteSpace(slug) ? "san-pham" : slug;
        }

        private async Task UpsertProductAttributesAsync(Product product, Dictionary<string, string> incomingAttributes, CancellationToken cancellationToken)
        {
            var attributeRepo = _unitOfWork.Repository<ProductAttribute>();
            var valueRepo = _unitOfWork.Repository<ProductAttributeValue>();
            var selectionRepo = _unitOfWork.Repository<ProductAttributeSelection>();

            var normalizedPairs = incomingAttributes
                .Where(x => !string.IsNullOrWhiteSpace(x.Key) && !string.IsNullOrWhiteSpace(x.Value))
                .Select(x => new
                {
                    Code = NormalizeAttributeCode(x.Key),
                    Name = BuildAttributeName(x.Key),
                    Value = x.Value.Trim()
                })
                .GroupBy(x => x.Code)
                .Select(g => g.First())
                .ToList();

            if (normalizedPairs.Count == 0)
            {
                return;
            }

            var codes = normalizedPairs.Select(x => x.Code).ToList();
            var existingAttributes = await attributeRepo.GetAllAsQueryable()
                .Where(x => codes.Contains(x.Code))
                .ToListAsync(cancellationToken);

            var attributeMap = existingAttributes.ToDictionary(x => x.Code, x => x);

            foreach (var pair in normalizedPairs)
            {
                if (!attributeMap.TryGetValue(pair.Code, out var attribute))
                {
                    attribute = new ProductAttribute
                    {
                        Code = pair.Code,
                        Name = pair.Name
                    };

                    await attributeRepo.AddAsync(attribute);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    attributeMap[pair.Code] = attribute;
                }
            }

            var attributeIds = attributeMap.Values.Select(x => x.Id).ToList();
            var existingValues = await valueRepo.GetAllAsQueryable()
                .Where(x => attributeIds.Contains(x.AttributeId))
                .ToListAsync(cancellationToken);

            var valueLookup = existingValues.ToDictionary(x => $"{x.AttributeId}::{x.Value}", x => x);
            var desired = new List<(int attributeId, int valueId)>();

            foreach (var pair in normalizedPairs)
            {
                var attribute = attributeMap[pair.Code];
                var valueKey = $"{attribute.Id}::{pair.Value}";

                if (!valueLookup.TryGetValue(valueKey, out var attributeValue))
                {
                    attributeValue = new ProductAttributeValue
                    {
                        AttributeId = attribute.Id,
                        Value = pair.Value
                    };

                    await valueRepo.AddAsync(attributeValue);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    valueLookup[valueKey] = attributeValue;
                }

                desired.Add((attribute.Id, attributeValue.Id));
            }

            selectionRepo.RemoveRange(product.AttributeSelections.ToList());

            foreach (var item in desired)
            {
                await selectionRepo.AddAsync(new ProductAttributeSelection
                {
                    ProductId = product.Id,
                    AttributeId = item.attributeId,
                    AttributeValueId = item.valueId
                });
            }
        }

        private async Task UpsertTierPricesAsync(Product product, List<SyncWordPressTierPriceInput> incomingTierPrices)
        {
            var normalizedTierPrices = incomingTierPrices
                .Where(x => x.Price >= 0)
                .Select(x => new ProductTierPrice
                {
                    ProductId = product.Id,
                    MinQuantity = x.MinQuantity <= 0 ? 1 : x.MinQuantity,
                    Price = x.Price
                })
                .OrderBy(x => x.MinQuantity)
                .ToList();

            if (normalizedTierPrices.Count == 0)
            {
                return;
            }

            var tierRepo = _unitOfWork.Repository<ProductTierPrice>();
            tierRepo.RemoveRange(product.TierPrices.ToList());

            foreach (var tier in normalizedTierPrices)
            {
                await tierRepo.AddAsync(tier);
            }
        }

        private static string NormalizeAttributeCode(string key)
        {
            var raw = key.Trim().ToLowerInvariant();
            raw = raw.StartsWith("pa_", StringComparison.Ordinal) ? raw[3..] : raw;

            var builder = new StringBuilder(raw.Length);
            foreach (var ch in raw)
            {
                if (char.IsLetterOrDigit(ch))
                {
                    builder.Append(char.ToUpperInvariant(ch));
                }
                else
                {
                    builder.Append('_');
                }
            }

            var normalized = builder.ToString();
            while (normalized.Contains("__", StringComparison.Ordinal))
            {
                normalized = normalized.Replace("__", "_", StringComparison.Ordinal);
            }

            return normalized.Trim('_');
        }

        private static string BuildAttributeName(string key)
        {
            var raw = key.Trim().ToLowerInvariant();
            raw = raw.StartsWith("pa_", StringComparison.Ordinal) ? raw[3..] : raw;
            raw = raw.Replace('-', ' ').Replace('_', ' ');
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(raw);
        }

        private async Task UpsertLevelDiscountsAsync(
            Product product,
            List<SyncWordPressLevelDiscountInput> incomingLevelDiscounts,
            CancellationToken cancellationToken)
        {
            var discountRepo = _unitOfWork.Repository<ProductLevelDiscount>();

            // Always mirror source: when payload explicitly contains role_base_prices, replace existing set.
            discountRepo.RemoveRange(product.LevelDiscounts.ToList());

            if (incomingLevelDiscounts.Count == 0)
            {
                return;
            }

            var normalized = incomingLevelDiscounts
                .Where(x => x.LevelRank > 0)
                .GroupBy(x => x.LevelRank)
                .Select(g => g.First())
                .Select(x => new SyncWordPressLevelDiscountInput
                {
                    LevelRank = x.LevelRank,
                    DiscountPercent = Math.Clamp(x.DiscountPercent, 0m, 100m),
                    IsActive = x.IsActive
                })
                .ToList();

            var ranks = normalized.Select(x => x.LevelRank).ToList();
            var levels = await _unitOfWork.Repository<UserLevel>()
                .GetAllAsQueryable()
                .Where(x => !x.IsDeleted && ranks.Contains(x.Rank))
                .ToListAsync(cancellationToken);

            var levelByRank = levels.ToDictionary(x => x.Rank, x => x);

            foreach (var item in normalized)
            {
                if (!levelByRank.TryGetValue(item.LevelRank, out var level))
                {
                    continue;
                }

                await discountRepo.AddAsync(new ProductLevelDiscount
                {
                    ProductId = product.Id,
                    UserLevelId = level.Id,
                    DiscountPercent = decimal.Round(item.DiscountPercent, 2, MidpointRounding.AwayFromZero),
                    IsActive = item.IsActive
                });
            }
        }
    }
}
