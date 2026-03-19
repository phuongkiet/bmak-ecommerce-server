namespace bmak_ecommerce.Application.Features.RoomScenes.Commands.DeleteRoomScene
{
    public class DeleteRoomSceneCommand
    {
        public int Id { get; }

        public DeleteRoomSceneCommand(int id)
        {
            Id = id;
        }
    }
}
