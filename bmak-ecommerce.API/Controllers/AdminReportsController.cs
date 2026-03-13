using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Reports.DTOs;
using bmak_ecommerce.Application.Features.Reports.Queries.GetProductReport;
using bmak_ecommerce.Application.Features.Reports.Queries.GetRevenueReport;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bmak_ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/admin/reports")]
    [Authorize(Roles = "Admin")]
    public class AdminReportsController : BaseApiController
    {
        private readonly IQueryHandler<GetRevenueReportQuery, RevenueReportDto> _getRevenueReportHandler;
        private readonly IQueryHandler<GetProductReportQuery, ProductReportDto> _getProductReportHandler;

        public AdminReportsController(
            IQueryHandler<GetRevenueReportQuery, RevenueReportDto> getRevenueReportHandler,
            IQueryHandler<GetProductReportQuery, ProductReportDto> getProductReportHandler)
        {
            _getRevenueReportHandler = getRevenueReportHandler;
            _getProductReportHandler = getProductReportHandler;
        }

        [HttpGet("revenue")]
        public async Task<ActionResult<ApiResponse<RevenueReportDto>>> GetRevenueReport(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var result = await _getRevenueReportHandler.Handle(new GetRevenueReportQuery(fromDate, toDate));
            return HandleResult(result);
        }

        [HttpGet("products")]
        public async Task<ActionResult<ApiResponse<ProductReportDto>>> GetProductReport(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int top = 10)
        {
            var result = await _getProductReportHandler.Handle(new GetProductReportQuery(fromDate, toDate, top));
            return HandleResult(result);
        }
    }
}
