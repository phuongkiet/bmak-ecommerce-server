using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Favorites.Commands.AddFavorite;
using bmak_ecommerce.Application.Features.Favorites.Commands.RemoveFavorite;
using bmak_ecommerce.Application.Features.Favorites.Dtos;
using bmak_ecommerce.Application.Features.Favorites.Queries.GetFavorites;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bmak_ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FavoritesController : BaseApiController
    {
        private readonly IQueryHandler<GetFavoritesQuery, List<FavoriteProductDto>> _getFavoritesHandler;
        private readonly ICommandHandler<AddFavoriteCommand, bool> _addFavoriteHandler;
        private readonly ICommandHandler<RemoveFavoriteCommand, bool> _removeFavoriteHandler;

        public FavoritesController(
            IQueryHandler<GetFavoritesQuery, List<FavoriteProductDto>> getFavoritesHandler,
            ICommandHandler<AddFavoriteCommand, bool> addFavoriteHandler,
            ICommandHandler<RemoveFavoriteCommand, bool> removeFavoriteHandler)
        {
            _getFavoritesHandler = getFavoritesHandler;
            _addFavoriteHandler = addFavoriteHandler;
            _removeFavoriteHandler = removeFavoriteHandler;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<FavoriteProductDto>>>> GetFavorites()
        {
            var result = await _getFavoritesHandler.Handle(new GetFavoritesQuery());
            return HandleResult(result);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<bool>>> AddFavorite([FromBody] AddFavoriteCommand command)
        {
            var result = await _addFavoriteHandler.Handle(command);
            return HandleResult(result);
        }

        [HttpDelete("{productId:int}")]
        public async Task<ActionResult<ApiResponse<bool>>> RemoveFavorite(int productId)
        {
            var result = await _removeFavoriteHandler.Handle(new RemoveFavoriteCommand(productId));
            return HandleResult(result);
        }
    }
}
