using bmak_ecommerce.Domain.Common;

namespace bmak_ecommerce.Domain.Entities.Visualizer
{
    public class RoomScene : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string ConfigJson { get; set; } = string.Empty;
        public string RoomLayerUrl { get; set; } = string.Empty;
        public string MattLayerUrl { get; set; } = string.Empty;
        public string GlossyLayerUrl { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
