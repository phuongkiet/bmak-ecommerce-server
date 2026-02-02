using bmak_ecommerce.API.Extensions;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Provinces.Dtos;
using bmak_ecommerce.Application.Features.Provinces.Queries;
using bmak_ecommerce.Application.Features.Wards.Dtos;
using bmak_ecommerce.Application.Features.Wards.Queries;
using bmak_ecommerce.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace bmak_ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WardController : BaseApiController
    {
        private readonly IQueryHandler<GetWardQuery, PagedList<WardDto>> _getWardHandler;

        public WardController(IQueryHandler<GetWardQuery, PagedList<WardDto>> getWardHandler)
        {
            _getWardHandler = getWardHandler;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedList<WardDto>>>> GetWardsByProvince([FromQuery] WardSpecParams specParams)
        {
            specParams.PageSize = 1000;
            var query = new GetWardQuery(specParams);

            var result = await _getWardHandler.Handle(query);

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

            return HandleResult(result);
        }
    }
}
