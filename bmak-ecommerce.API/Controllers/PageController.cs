using bmak_ecommerce.API.Extensions;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Features.Orders.Queries.GetAllOrders;
using bmak_ecommerce.Application.Features.Pages.Commands.CreatePage;
using bmak_ecommerce.Application.Features.Pages.DTOs;
using bmak_ecommerce.Application.Features.Pages.Queries.GetAllPages;
using bmak_ecommerce.Application.Features.Pages.Queries.GetPageDetail;
using bmak_ecommerce.Application.Features.Products.Commands.CreateProduct;
using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using bmak_ecommerce.Application.Features.Products.DTOs.Sale;
using bmak_ecommerce.Application.Features.Products.Queries.Products.GetProductById;
using bmak_ecommerce.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace bmak_ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PageController : BaseApiController
    {
        private readonly IQueryHandler<GetPageQuery, PagedList<PageSummaryDto>> _getPagesHandler;
        private readonly IQueryHandler<GetPageDetailQuery, PageDto> _getPageDetailHandler;
        private readonly ICommandHandler<CreatePageCommand, string> _createPageHandler;
        public PageController(IQueryHandler<GetPageQuery, PagedList<PageSummaryDto>> getPagesHandler,
                IQueryHandler<GetPageDetailQuery, PageDto> getPageDetailHandler,
                ICommandHandler<CreatePageCommand, string> createPageHandler)
        {
            _getPagesHandler = getPagesHandler;
            _getPageDetailHandler = getPageDetailHandler;
            _createPageHandler = createPageHandler;
        }
        // GET: api/page
        [HttpGet]
        public async Task<ActionResult<PagedList<PageSummaryDto>>> GetPages([FromQuery] PageSpecParams specParams)
        {
            var query = new GetPageQuery(specParams);

            var result = await _getPagesHandler.Handle(query);

            // Xử lý Pagination Header nếu thành công
            if (result.IsSuccess && result.Value != null)
            {
                Response.AddPaginationHeader(
                    result.Value.PageIndex,
                    result.Value.PageSize,
                    result.Value.TotalCount,
                    result.Value.TotalPages
                );
            }

            // Dùng HandleResult của BaseApiController
            return HandleResult(result);
        }

        [HttpGet("{slug}")]
        public async Task<ActionResult<PageDto>> GetBySlug(string slug)
        {
            var query = new GetPageDetailQuery(slug);

            // Gọi Handler -> nhận Result<ProductDto?>
            var result = await _getPageDetailHandler.Handle(query, CancellationToken.None);

            // Trả về HTTP 200 (Success) hoặc 404 (Failure)
            return HandleResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePage([FromBody] CreatePageCommand command)
        {
            var result = await _createPageHandler.Handle(command);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetBySlug), new { slug = result.Value }, new { slug = result.Value });
            }

            return BadRequest(new { message = result.Error });
        }
    }
}
