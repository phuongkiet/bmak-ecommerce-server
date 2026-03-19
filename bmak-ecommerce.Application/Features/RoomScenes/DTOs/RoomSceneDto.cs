namespace bmak_ecommerce.Application.Features.RoomScenes.DTOs
{
    public class RoomSceneDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string ConfigJson { get; set; } = string.Empty;
        public string RoomLayerUrl { get; set; } = string.Empty;
        public string MattLayerUrl { get; set; } = string.Empty;
        public string GlossyLayerUrl { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
