namespace bmak_ecommerce.Application.Features.NewsCategories.Commands.UpdateNewsCategory
{
    public class UpdateNewsCategoryCommand
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
