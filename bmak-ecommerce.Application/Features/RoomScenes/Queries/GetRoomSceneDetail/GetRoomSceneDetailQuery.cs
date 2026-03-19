namespace bmak_ecommerce.Application.Features.RoomScenes.Queries.GetRoomSceneDetail
{
    public class GetRoomSceneDetailQuery
    {
        public int Id { get; }
        public bool IncludeInactive { get; }

        public GetRoomSceneDetailQuery(int id, bool includeInactive = false)
        {
            Id = id;
            IncludeInactive = includeInactive;
        }
    }
}
