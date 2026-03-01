namespace bmak_ecommerce.Application.Features.NewsPosts.Commands.UpdateNewsPost
{
    public class UpdateNewsPostCommand
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int? AuthorId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public bool IsPublished { get; set; }
    }
}
