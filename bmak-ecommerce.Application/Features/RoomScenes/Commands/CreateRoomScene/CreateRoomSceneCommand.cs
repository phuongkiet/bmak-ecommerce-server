namespace bmak_ecommerce.Application.Features.RoomScenes.Commands.CreateRoomScene
{
    public class CreateRoomSceneCommand
    {
        public string Title { get; set; } = string.Empty;
        public string ConfigJson { get; set; } = string.Empty;
        public string RoomLayerUrl { get; set; } = string.Empty;
        public string MattLayerUrl { get; set; } = string.Empty;
        public string GlossyLayerUrl { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
