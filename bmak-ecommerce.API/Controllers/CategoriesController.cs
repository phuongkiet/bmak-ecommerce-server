using bmak_ecommerce.Application.Features.Categories.Commands.CreateCategory;
using bmak_ecommerce.Application.Features.Categories.Queries;
using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using bmak_ecommerce.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace bmak_ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/categories?pageIndex=1&pageSize=10&search=gạch&parentId=1
        [HttpGet]
        public async Task<ActionResult<PagedList<CategoryDto>>> GetCategories([FromQuery] CategorySpecParams categoryParams)
        {
            var query = new GetCategoriesQuery(categoryParams);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // POST: api/categories
        [HttpPost]
        public async Task<ActionResult<int>> CreateCategory([FromBody] CreateCategoryCommand command)
        {
            var categoryId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetCategories), new { id = categoryId }, categoryId);
        }

        // PUT: api/categories/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> UpdateCategory(int id, [FromBody] bmak_ecommerce.Application.Features.Categories.Commands.UpdateCategory.UpdateCategoryCommand command)
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


