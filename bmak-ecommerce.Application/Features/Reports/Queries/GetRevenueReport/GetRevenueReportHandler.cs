using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Reports.DTOs;
using bmak_ecommerce.Domain.Interfaces;
using bmak_ecommerce.Domain.Models;

namespace bmak_ecommerce.Application.Features.Reports.Queries.GetRevenueReport
{
    [AutoRegister]
    public class GetRevenueReportHandler : IQueryHandler<GetRevenueReportQuery, RevenueReportDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRevenueReportHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<RevenueReportDto>> Handle(GetRevenueReportQuery query, CancellationToken cancellationToken = default)
        {
            if (query.FromDate.HasValue && query.ToDate.HasValue && query.FromDate > query.ToDate)
            {
                return Result<RevenueReportDto>.Failure("FromDate must be less than or equal to ToDate");
            }

            var report = await _unitOfWork.Orders.GetRevenueReportAsync(query.FromDate, query.ToDate, cancellationToken);

            var result = new RevenueReportDto
            {
                TotalRevenue = report.TotalRevenue,
                TotalOrders = report.TotalOrders,
                AverageOrderValue = report.AverageOrderValue,
                RevenueByDate = report.RevenueByDate.Select(x => new RevenueByDateDto
                {
                    Date = x.Date,
                    Revenue = x.Revenue,
                    Orders = x.Orders
                }).ToList()
            };

            return Result<RevenueReportDto>.Success(result);
        }
    }
}
