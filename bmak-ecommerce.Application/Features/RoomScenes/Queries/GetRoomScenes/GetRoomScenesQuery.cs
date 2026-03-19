namespace bmak_ecommerce.Application.Features.RoomScenes.Queries.GetRoomScenes
{
    public class GetRoomScenesQuery
    {
        public bool IncludeInactive { get; }

        public GetRoomScenesQuery(bool includeInactive = false)
        {
            IncludeInactive = includeInactive;
        }
    }
}
