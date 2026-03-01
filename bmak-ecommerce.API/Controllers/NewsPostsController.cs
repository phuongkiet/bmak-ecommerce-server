using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.NewsPosts.Commands.CreateNewsPost;
using bmak_ecommerce.Application.Features.NewsPosts.Commands.DeleteNewsPost;
using bmak_ecommerce.Application.Features.NewsPosts.Commands.UpdateNewsPost;
using bmak_ecommerce.Application.Features.NewsPosts.DTOs;
using bmak_ecommerce.Application.Features.NewsPosts.Queries;
using Microsoft.AspNetCore.Mvc;

namespace bmak_ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsPostsController : BaseApiController
    {
        private readonly IQueryHandler<GetNewsPostsQuery, List<NewsPostSummaryDto>> _getNewsPostsHandler;
        private readonly IQueryHandler<GetNewsPostDetailQuery, NewsPostDto> _getNewsPostDetailHandler;
        private readonly ICommandHandler<CreateNewsPostCommand, int> _createNewsPostHandler;
        private readonly ICommandHandler<UpdateNewsPostCommand, bool> _updateNewsPostHandler;
        private readonly ICommandHandler<DeleteNewsPostCommand, bool> _deleteNewsPostHandler;

        public NewsPostsController(
            IQueryHandler<GetNewsPostsQuery, List<NewsPostSummaryDto>> getNewsPostsHandler,
            IQueryHandler<GetNewsPostDetailQuery, NewsPostDto> getNewsPostDetailHandler,
            ICommandHandler<CreateNewsPostCommand, int> createNewsPostHandler,
            ICommandHandler<UpdateNewsPostCommand, bool> updateNewsPostHandler,
            ICommandHandler<DeleteNewsPostCommand, bool> deleteNewsPostHandler)
        {
            _getNewsPostsHandler = getNewsPostsHandler;
            _getNewsPostDetailHandler = getNewsPostDetailHandler;
            _createNewsPostHandler = createNewsPostHandler;
            _updateNewsPostHandler = updateNewsPostHandler;
            _deleteNewsPostHandler = deleteNewsPostHandler;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<NewsPostSummaryDto>>>> GetNewsPosts()
        {
            var result = await _getNewsPostsHandler.Handle(new GetNewsPostsQuery());
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<NewsPostDto>>> GetNewsPost(int id)
        {
            var result = await _getNewsPostDetailHandler.Handle(new GetNewsPostDetailQuery(id));
            return HandleResult(result);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> CreateNewsPost([FromBody] CreateNewsPostCommand command)
        {
            var result = await _createNewsPostHandler.Handle(command);
            return HandleResult(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateNewsPost(int id, [FromBody] UpdateNewsPostCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest(ApiResponse<bool>.Failure("ID trong URL và body không khớp"));
            }

            var result = await _updateNewsPostHandler.Handle(command);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteNewsPost(int id)
        {
            var result = await _deleteNewsPostHandler.Handle(new DeleteNewsPostCommand { Id = id });
            return HandleResult(result);
        }
    }
}
