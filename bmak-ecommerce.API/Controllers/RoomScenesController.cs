using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.RoomScenes.DTOs;
using bmak_ecommerce.Application.Features.RoomScenes.Queries.GetRoomSceneDetail;
using bmak_ecommerce.Application.Features.RoomScenes.Queries.GetRoomScenes;
using bmak_ecommerce.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bmak_ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/room-scenes")]
    [Authorize]
    public class RoomScenesController : BaseApiController
    {
        private readonly IQueryHandler<GetRoomScenesQuery, List<RoomSceneDto>> _getRoomScenesHandler;
        private readonly IQueryHandler<GetRoomSceneDetailQuery, RoomSceneDto> _getRoomSceneDetailHandler;

        public RoomScenesController(
            IQueryHandler<GetRoomScenesQuery, List<RoomSceneDto>> getRoomScenesHandler,
            IQueryHandler<GetRoomSceneDetailQuery, RoomSceneDto> getRoomSceneDetailHandler)
        {
            _getRoomScenesHandler = getRoomScenesHandler;
            _getRoomSceneDetailHandler = getRoomSceneDetailHandler;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<RoomSceneDto>>>> GetRoomScenes()
        {
            var result = await _getRoomScenesHandler.Handle(new GetRoomScenesQuery());
            return HandleResult(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<RoomSceneDto>>> GetRoomSceneDetail(int id)
        {
            var result = await _getRoomSceneDetailHandler.Handle(new GetRoomSceneDetailQuery(id));
            return HandleResult(result);
        }
    }
}
