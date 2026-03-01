namespace bmak_ecommerce.Application.Features.NewsCategories.DTOs
{
    public class NewsCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
