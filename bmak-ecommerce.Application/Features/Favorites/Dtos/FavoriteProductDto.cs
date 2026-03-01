namespace bmak_ecommerce.Application.Features.Favorites.Dtos
{
    public class FavoriteProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }
        public string Thumbnail { get; set; } = string.Empty;
        public DateTime AddedAt { get; set; }
    }
}
