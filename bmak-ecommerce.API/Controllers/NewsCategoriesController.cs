using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.NewsCategories.Commands.CreateNewsCategory;
using bmak_ecommerce.Application.Features.NewsCategories.Commands.DeleteNewsCategory;
using bmak_ecommerce.Application.Features.NewsCategories.Commands.UpdateNewsCategory;
using bmak_ecommerce.Application.Features.NewsCategories.DTOs;
using bmak_ecommerce.Application.Features.NewsCategories.Queries;
using Microsoft.AspNetCore.Mvc;

namespace bmak_ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsCategoriesController : BaseApiController
    {
        private readonly IQueryHandler<GetNewsCategoriesQuery, List<NewsCategoryDto>> _getCategoriesHandler;
        private readonly IQueryHandler<GetNewsCategoryDetailQuery, NewsCategoryDto> _getCategoryDetailHandler;
        private readonly ICommandHandler<CreateNewsCategoryCommand, int> _createCategoryHandler;
        private readonly ICommandHandler<UpdateNewsCategoryCommand, bool> _updateCategoryHandler;
        private readonly ICommandHandler<DeleteNewsCategoryCommand, bool> _deleteCategoryHandler;

        public NewsCategoriesController(
            IQueryHandler<GetNewsCategoriesQuery, List<NewsCategoryDto>> getCategoriesHandler,
            IQueryHandler<GetNewsCategoryDetailQuery, NewsCategoryDto> getCategoryDetailHandler,
            ICommandHandler<CreateNewsCategoryCommand, int> createCategoryHandler,
            ICommandHandler<UpdateNewsCategoryCommand, bool> updateCategoryHandler,
            ICommandHandler<DeleteNewsCategoryCommand, bool> deleteCategoryHandler)
        {
            _getCategoriesHandler = getCategoriesHandler;
            _getCategoryDetailHandler = getCategoryDetailHandler;
            _createCategoryHandler = createCategoryHandler;
            _updateCategoryHandler = updateCategoryHandler;
            _deleteCategoryHandler = deleteCategoryHandler;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<NewsCategoryDto>>>> GetNewsCategories()
        {
            var result = await _getCategoriesHandler.Handle(new GetNewsCategoriesQuery());
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<NewsCategoryDto>>> GetNewsCategory(int id)
        {
            var result = await _getCategoryDetailHandler.Handle(new GetNewsCategoryDetailQuery(id));
            return HandleResult(result);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> CreateNewsCategory([FromBody] CreateNewsCategoryCommand command)
        {
            var result = await _createCategoryHandler.Handle(command);
            return HandleResult(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateNewsCategory(int id, [FromBody] UpdateNewsCategoryCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest(ApiResponse<bool>.Failure("ID trong URL và body không khớp"));
            }

            var result = await _updateCategoryHandler.Handle(command);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteNewsCategory(int id)
        {
            var result = await _deleteCategoryHandler.Handle(new DeleteNewsCategoryCommand { Id = id });
            return HandleResult(result);
        }
    }
}
