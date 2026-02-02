using bmak_ecommerce.API.Extensions;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Orders.Queries.GetAllOrders;
using bmak_ecommerce.Application.Features.Provinces.Dtos;
using bmak_ecommerce.Application.Features.Provinces.Queries;
using bmak_ecommerce.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace bmak_ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvinceController : BaseApiController
    {
        private readonly IQueryHandler<GetProvinceQuery, PagedList<ProvinceDto>> _getProvinceHandler;

        public ProvinceController(IQueryHandler<GetProvinceQuery, PagedList<ProvinceDto>> getProvinceHandler)
        {
            _getProvinceHandler = getProvinceHandler;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedList<ProvinceDto>>>> GetProvinces([FromQuery] ProvinceSpecParams specParams)
        {
            specParams.PageSize = 1000;
            var query = new GetProvinceQuery(specParams);

            var result = await _getProvinceHandler.Handle(query);

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
