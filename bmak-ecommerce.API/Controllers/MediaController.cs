using bmak_ecommerce.API.Extensions;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Media.Commands.DeleteImage;
using bmak_ecommerce.Application.Features.Media.Commands.UploadImage;
using bmak_ecommerce.Application.Features.Media.DTOs;
using bmak_ecommerce.Application.Features.Media.Queries.GetImages;
using bmak_ecommerce.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace bmak_ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : BaseApiController
    {
        private readonly ICommandHandler<UploadImageCommand, AppImageDto> _uploadHandler;
        private readonly IQueryHandler<GetImagesQuery, PagedList<AppImageDto>> _getImagesHandler;
        private readonly ICommandHandler<DeleteImageCommand, bool> _deleteHandler;

        public MediaController(ICommandHandler<UploadImageCommand, AppImageDto> uploadHandler,
            IQueryHandler<GetImagesQuery, PagedList<AppImageDto>> getImagesHandler,
            ICommandHandler<DeleteImageCommand, bool> deleteHandler)
        {
            _uploadHandler = uploadHandler;
            _getImagesHandler = getImagesHandler;
            _deleteHandler = deleteHandler;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedList<AppImageDto>>>> GetImages([FromQuery] GetImagesQuery query)
        {
            var result = await _getImagesHandler.Handle(query);

            if (result.IsSuccess)
                Response.AddPaginationHeader(result.Value.PageIndex, result.Value.PageSize, result.Value.TotalCount, result.Value.TotalPages);

            return HandleResult(result);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<AppImageDto>>> Upload([FromForm] UploadImageRequest request)
        {
            var command = new UploadImageCommand { File = request.File };
            var result = await _uploadHandler.Handle(command);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            var command = new DeleteImageCommand { Id = id };
            var result = await _deleteHandler.Handle(command);
            return HandleResult(result);
        }
    }
}
