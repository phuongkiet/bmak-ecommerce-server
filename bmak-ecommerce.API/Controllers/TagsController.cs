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
        public async Task<ActionResult<List<TagDto>>> GetTags()
        {
            var query = new GetTagsQuery();
            var tags = await _mediator.Send(query);
            return Ok(tags);
        }

        // POST: api/tags
        [HttpPost]
        public async Task<ActionResult<int>> CreateTag([FromBody] CreateTagCommand command)
        {
            var tagId = await _mediator.Send(command);
            return CreatedAtAction(nameof(CreateTag), new { id = tagId }, tagId);
        }

        // PUT: api/tags/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> UpdateTag(int id, [FromBody] UpdateTagCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID trong URL và body không khớp");
            }

            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}

