using bmak_ecommerce.API.Extensions;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Categories.Admin.Commands.CreateAdminCategory;
using bmak_ecommerce.Application.Features.Categories.Admin.Commands.DeleteAdminCategory;
using bmak_ecommerce.Application.Features.Categories.Admin.Commands.UpdateAdminCategory;
using bmak_ecommerce.Application.Features.Categories.Admin.Queries.GetAdminCategories;
using bmak_ecommerce.Application.Features.Categories.Admin.Queries.GetAdminCategoryDetail;
using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using bmak_ecommerce.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bmak_ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/admin/categories")]
    [Authorize(Roles = "Admin")]
    public class AdminCategoriesController : BaseApiController
    {
        private readonly IQueryHandler<GetAdminCategoriesQuery, PagedList<CategoryDto>> _getCategoriesHandler;
        private readonly IQueryHandler<GetAdminCategoryDetailQuery, CategoryDto> _getCategoryDetailHandler;
        private readonly ICommandHandler<CreateAdminCategoryCommand, int> _createCategoryHandler;
        private readonly ICommandHandler<UpdateAdminCategoryCommand, bool> _updateCategoryHandler;
        private readonly ICommandHandler<DeleteAdminCategoryCommand, bool> _deleteCategoryHandler;

        public AdminCategoriesController(
            IQueryHandler<GetAdminCategoriesQuery, PagedList<CategoryDto>> getCategoriesHandler,
            IQueryHandler<GetAdminCategoryDetailQuery, CategoryDto> getCategoryDetailHandler,
            ICommandHandler<CreateAdminCategoryCommand, int> createCategoryHandler,
            ICommandHandler<UpdateAdminCategoryCommand, bool> updateCategoryHandler,
            ICommandHandler<DeleteAdminCategoryCommand, bool> deleteCategoryHandler)
        {
            _getCategoriesHandler = getCategoriesHandler;
            _getCategoryDetailHandler = getCategoryDetailHandler;
            _createCategoryHandler = createCategoryHandler;
            _updateCategoryHandler = updateCategoryHandler;
            _deleteCategoryHandler = deleteCategoryHandler;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedList<CategoryDto>>>> GetCategories([FromQuery] CategorySpecParams specParams)
        {
            var result = await _getCategoriesHandler.Handle(new GetAdminCategoriesQuery(specParams));

            if (result.IsSuccess && result.Value != null)
            {
                Response.AddPaginationHeader(
                    result.Value.PageIndex,
                    result.Value.PageSize,
                    result.Value.TotalCount,
                    result.Value.TotalPages
                );
            }

            return HandleResult(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> GetCategoryDetail(int id)
        {
            var result = await _getCategoryDetailHandler.Handle(new GetAdminCategoryDetailQuery(id));
            return HandleResult(result);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> CreateCategory([FromBody] CreateAdminCategoryCommand command)
        {
            var result = await _createCategoryHandler.Handle(command);
            return HandleResult(result);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateCategory(int id, [FromBody] UpdateAdminCategoryCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest(ApiResponse<bool>.Failure("ID trong URL va body khong khop"));
            }

            var result = await _updateCategoryHandler.Handle(command);
            return HandleResult(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteCategory(int id)
        {
            var result = await _deleteCategoryHandler.Handle(new DeleteAdminCategoryCommand(id));
            return HandleResult(result);
        }
    }
}
