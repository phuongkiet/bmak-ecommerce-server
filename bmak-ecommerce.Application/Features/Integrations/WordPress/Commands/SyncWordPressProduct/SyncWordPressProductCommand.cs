using bmak_ecommerce.Application.Common.Models;

namespace bmak_ecommerce.Application.Features.Integrations.WordPress.Commands.SyncWordPressProduct
{
    public class SyncWordPressProductCommand
    {
        public int? WordPressId { get; set; }
        public string? Name { get; set; }
        public string? Sku { get; set; }
        public string? Slug { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        public decimal? BasePrice { get; set; }
        public decimal? SalePrice { get; set; }
        public bool? IsActive { get; set; }

        // extra_fields
        public string? SalesUnit { get; set; }
        public string? PriceUnit { get; set; }
        public decimal? Thickness { get; set; }
        public int? BoxQuantity { get; set; }
        public int? Random { get; set; }

        // Optional stock/weight sync
        public float? Weight { get; set; }
        public bool? ManageStock { get; set; }
        public bool? AllowBackorder { get; set; }

        // pa_* from variation/default_attributes
        public Dictionary<string, string> Attributes { get; set; } = new();

        // tier_prices -> ProductTierPrice
        public List<SyncWordPressTierPriceInput> TierPrices { get; set; } = new();

        // role_base_prices -> ProductLevelDiscount (percentage)
        public List<SyncWordPressLevelDiscountInput> LevelDiscounts { get; set; } = new();
        public bool ReplaceLevelDiscounts { get; set; }
    }

    public class SyncWordPressTierPriceInput
    {
        public float MinQuantity { get; set; }
        public decimal Price { get; set; }
    }

    public class SyncWordPressLevelDiscountInput
    {
        public int LevelRank { get; set; }
        public decimal DiscountPercent { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class SyncWordPressProductResult
    {
        public int ProductId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? Sku { get; set; }
        public string? ThumbnailUrl { get; set; }
    }
}
