using bmak_ecommerce.Application.Common.Models;
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
        public async Task<ActionResult<ApiResponse<PagedList<CategoryDto>>>> GetCategories([FromQuery] CategorySpecParams categoryParams)
        {
            try
            {
                var query = new GetCategoriesQuery(categoryParams);
                var result = await _mediator.Send(query);
                return Ok(ApiResponse<PagedList<CategoryDto>>.Success(result, "Categories retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<PagedList<CategoryDto>>.Failure(ex.Message));
            }
        }

        // POST: api/categories
        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> CreateCategory([FromBody] CreateCategoryCommand command)
        {
            try
            {
                var categoryId = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetCategories), new { id = categoryId }, ApiResponse<int>.Success(categoryId, "Category created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<int>.Failure(ex.Message));
            }
        }

        // PUT: api/categories/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateCategory(int id, [FromBody] bmak_ecommerce.Application.Features.Categories.Commands.UpdateCategory.UpdateCategoryCommand command)
        {
            try
            {
                if (id != command.Id)
                {
                    return BadRequest(ApiResponse<bool>.Failure("ID trong URL và body không khớp"));
                }

                var result = await _mediator.Send(command);
                return Ok(ApiResponse<bool>.Success(result, "Category updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.Failure(ex.Message));
            }
        }
    }
}


