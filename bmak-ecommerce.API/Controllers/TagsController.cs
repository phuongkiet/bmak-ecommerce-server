using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using bmak_ecommerce.Application.Features.Tags.Commands.CreateTag;
using bmak_ecommerce.Application.Features.Tags.Commands.UpdateTag;
using bmak_ecommerce.Application.Features.Tags.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace bmak_ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TagsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/tags
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<TagDto>>>> GetTags()
        {
            try
            {
                var query = new GetTagsQuery();
                var tags = await _mediator.Send(query);
                return Ok(ApiResponse<List<TagDto>>.Success(tags, "Tags retrieved successfully"));
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
                var tagId = await _mediator.Send(command);
                return CreatedAtAction(nameof(CreateTag), new { id = tagId }, ApiResponse<int>.Success(tagId, "Tag created successfully"));
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

                var result = await _mediator.Send(command);
                return Ok(ApiResponse<bool>.Success(result, "Tag updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.Failure(ex.Message));
            }
        }
    }
}

