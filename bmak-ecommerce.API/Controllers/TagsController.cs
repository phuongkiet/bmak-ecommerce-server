using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using bmak_ecommerce.Application.Features.Tags.Commands.CreateTag;
using bmak_ecommerce.Application.Features.Tags.Commands.DeleteTag;
using bmak_ecommerce.Application.Features.Tags.Commands.UpdateTag;
using bmak_ecommerce.Application.Features.Tags.Queries;
using MassTransit.SagaStateMachine;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace bmak_ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        // Khai báo các Handler cụ thể thay vì Mediator
        private readonly IQueryHandler<GetTagsQuery, List<TagDto>> _getTagsHandler;
        private readonly ICommandHandler<CreateTagCommand, int> _createTagHandler;
        private readonly ICommandHandler<UpdateTagCommand, bool> _updateTagHandler;
        private readonly ICommandHandler<DeleteTagCommand, bool> _deleteTagHandler;

        // Inject từng Handler vào Constructor
        public TagsController(
            IQueryHandler<GetTagsQuery, List<TagDto>> getTagsHandler,
            ICommandHandler<CreateTagCommand, int> createTagHandler,
            ICommandHandler<UpdateTagCommand, bool> updateTagHandler,
            ICommandHandler<DeleteTagCommand, bool> deleteTagHandler)
        {
            _getTagsHandler = getTagsHandler;
            _createTagHandler = createTagHandler;
            _updateTagHandler = updateTagHandler;
            _deleteTagHandler = deleteTagHandler;
        }

        // GET: api/tags
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<TagDto>>>> GetTags()
        {
            try
            {
                var query = new GetTagsQuery();
                // Gọi trực tiếp hàm Handle của handler
                var result = await _getTagsHandler.Handle(query, default);

                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<List<TagDto>>.Success(result.Value, "Tags retrieved successfully"));
                }
                return BadRequest(ApiResponse<List<TagDto>>.Failure(result.Error));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<TagDto>>.Failure(ex.Message));
            }
        }

        // POST: api/tags
        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> CreateTag([FromBody] CreateTagCommand command)
        {
            try
            {
                // Gọi handler tạo
                var result = await _createTagHandler.Handle(command, default);

                if (result.IsSuccess)
                {
                    return CreatedAtAction(nameof(CreateTag), new { id = result.Value }, ApiResponse<int>.Success(result.Value, "Tag created successfully"));
                }
                return BadRequest(ApiResponse<int>.Failure(result.Error));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<int>.Failure(ex.Message));
            }
        }

        // PUT: api/tags/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateTag(int id, [FromBody] UpdateTagCommand command)
        {
            try
            {
                if (id != command.Id)
                {
                    return BadRequest(ApiResponse<bool>.Failure("ID trong URL và body không khớp"));
                }

                // Gọi handler update
                var result = await _updateTagHandler.Handle(command, default);

                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<bool>.Success(result.Value, "Tag updated successfully"));
                }
                return BadRequest(ApiResponse<bool>.Failure(result.Error));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.Failure(ex.Message));
            }
        }

        // DELETE: api/tags/{id}
        // (Bổ sung thêm action này vì thấy bạn có inject _deleteTagHandler)
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteTag(int id)
        {
            try
            {
                var command = new DeleteTagCommand { Id = id };
                var result = await _deleteTagHandler.Handle(command, default);

                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<bool>.Success(result.Value, "Tag deleted successfully"));
                }
                return BadRequest(ApiResponse<bool>.Failure(result.Error));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.Failure(ex.Message));
            }
        }
    }
}

