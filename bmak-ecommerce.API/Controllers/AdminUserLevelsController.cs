using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Products.Commands.UpsertProductLevelDiscounts;
using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using bmak_ecommerce.Application.Features.Products.Queries.Products.GetProductLevelDiscounts;
using bmak_ecommerce.Application.Features.Users.Commands.SetUserLevel;
using bmak_ecommerce.Application.Features.Users.Dtos;
using bmak_ecommerce.Application.Features.Users.Queries.GetUserLevels;
using bmak_ecommerce.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bmak_ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/admin/user-levels")]
    [Authorize(Roles = "Admin")]
    public class AdminUserLevelsController : BaseApiController
    {
        private readonly IQueryHandler<GetUserLevelsQuery, List<UserLevelDto>> _getUserLevelsHandler;
        private readonly ICommandHandler<SetUserLevelCommand, bool> _setUserLevelHandler;
        private readonly IQueryHandler<GetProductLevelDiscountsQuery, List<ProductLevelDiscountDto>> _getProductLevelDiscountsHandler;
        private readonly ICommandHandler<UpsertProductLevelDiscountsCommand, bool> _upsertProductLevelDiscountsHandler;

        public AdminUserLevelsController(
            IQueryHandler<GetUserLevelsQuery, List<UserLevelDto>> getUserLevelsHandler,
            ICommandHandler<SetUserLevelCommand, bool> setUserLevelHandler,
            IQueryHandler<GetProductLevelDiscountsQuery, List<ProductLevelDiscountDto>> getProductLevelDiscountsHandler,
            ICommandHandler<UpsertProductLevelDiscountsCommand, bool> upsertProductLevelDiscountsHandler)
        {
            _getUserLevelsHandler = getUserLevelsHandler;
            _setUserLevelHandler = setUserLevelHandler;
            _getProductLevelDiscountsHandler = getProductLevelDiscountsHandler;
            _upsertProductLevelDiscountsHandler = upsertProductLevelDiscountsHandler;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<UserLevelDto>>>> GetUserLevels()
        {
            var result = await _getUserLevelsHandler.Handle(new GetUserLevelsQuery());
            return HandleResult(result);
        }

        [HttpPatch("users/{userId:int}")]
        public async Task<ActionResult<ApiResponse<bool>>> SetUserLevel(int userId, [FromBody] SetUserLevelCommand command)
        {
            command.UserId = userId;
            var result = await _setUserLevelHandler.Handle(command);
            return HandleResult(result);
        }

        [HttpGet("products/{productId:int}/discounts")]
        public async Task<ActionResult<ApiResponse<List<ProductLevelDiscountDto>>>> GetProductLevelDiscounts(int productId)
        {
            var result = await _getProductLevelDiscountsHandler.Handle(new GetProductLevelDiscountsQuery(productId));
            return HandleResult(result);
        }

        [HttpPut("products/{productId:int}/discounts")]
        public async Task<ActionResult<ApiResponse<bool>>> UpsertProductLevelDiscounts(
            int productId,
            [FromBody] UpsertProductLevelDiscountsCommand command)
        {
            command.ProductId = productId;
            var result = await _upsertProductLevelDiscountsHandler.Handle(command);
            return HandleResult(result);
        }
    }
}
