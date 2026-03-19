using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.RoomScenes.Commands.CreateRoomScene;
using bmak_ecommerce.Application.Features.RoomScenes.Commands.DeleteRoomScene;
using bmak_ecommerce.Application.Features.RoomScenes.Commands.UpdateRoomScene;
using bmak_ecommerce.Application.Features.RoomScenes.DTOs;
using bmak_ecommerce.Application.Features.RoomScenes.Queries.GetRoomSceneDetail;
using bmak_ecommerce.Application.Features.RoomScenes.Queries.GetRoomScenes;
using bmak_ecommerce.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bmak_ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/admin/room-scenes")]
    [Authorize(Roles = "Admin")]
    public class AdminRoomScenesController : BaseApiController
    {
        private readonly IQueryHandler<GetRoomScenesQuery, List<RoomSceneDto>> _getRoomScenesHandler;
        private readonly IQueryHandler<GetRoomSceneDetailQuery, RoomSceneDto> _getRoomSceneDetailHandler;
        private readonly ICommandHandler<CreateRoomSceneCommand, int> _createRoomSceneHandler;
        private readonly ICommandHandler<UpdateRoomSceneCommand, bool> _updateRoomSceneHandler;
        private readonly ICommandHandler<DeleteRoomSceneCommand, bool> _deleteRoomSceneHandler;

        public AdminRoomScenesController(
            IQueryHandler<GetRoomScenesQuery, List<RoomSceneDto>> getRoomScenesHandler,
            IQueryHandler<GetRoomSceneDetailQuery, RoomSceneDto> getRoomSceneDetailHandler,
            ICommandHandler<CreateRoomSceneCommand, int> createRoomSceneHandler,
            ICommandHandler<UpdateRoomSceneCommand, bool> updateRoomSceneHandler,
            ICommandHandler<DeleteRoomSceneCommand, bool> deleteRoomSceneHandler)
        {
            _getRoomScenesHandler = getRoomScenesHandler;
            _getRoomSceneDetailHandler = getRoomSceneDetailHandler;
            _createRoomSceneHandler = createRoomSceneHandler;
            _updateRoomSceneHandler = updateRoomSceneHandler;
            _deleteRoomSceneHandler = deleteRoomSceneHandler;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<RoomSceneDto>>>> GetRoomScenes()
        {
            var result = await _getRoomScenesHandler.Handle(new GetRoomScenesQuery(includeInactive: true));
            return HandleResult(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<RoomSceneDto>>> GetRoomSceneDetail(int id)
        {
            var result = await _getRoomSceneDetailHandler.Handle(new GetRoomSceneDetailQuery(id, includeInactive: true));
            return HandleResult(result);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> CreateRoomScene([FromBody] CreateRoomSceneCommand command)
        {
            var result = await _createRoomSceneHandler.Handle(command);
            return HandleResult(result);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateRoomScene(int id, [FromBody] UpdateRoomSceneCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest(ApiResponse<bool>.Failure("ID trong URL va body khong khop"));
            }

            var result = await _updateRoomSceneHandler.Handle(command);
            return HandleResult(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteRoomScene(int id)
        {
            var result = await _deleteRoomSceneHandler.Handle(new DeleteRoomSceneCommand(id));
            return HandleResult(result);
        }
    }
}
