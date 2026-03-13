using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Reports.DTOs;
using bmak_ecommerce.Domain.Interfaces;
using bmak_ecommerce.Domain.Models;

namespace bmak_ecommerce.Application.Features.Reports.Queries.GetProductReport
{
    [AutoRegister]
    public class GetProductReportHandler : IQueryHandler<GetProductReportQuery, ProductReportDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProductReportHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ProductReportDto>> Handle(GetProductReportQuery query, CancellationToken cancellationToken = default)
        {
            if (query.FromDate.HasValue && query.ToDate.HasValue && query.FromDate > query.ToDate)
            {
                return Result<ProductReportDto>.Failure("FromDate must be less than or equal to ToDate");
            }

            var report = await _unitOfWork.Orders.GetProductReportAsync(query.FromDate, query.ToDate, query.Top, cancellationToken);

            var result = new ProductReportDto
            {
                TotalProductsSold = report.TotalProductsSold,
                TotalRevenue = report.TotalRevenue,
                TopProducts = report.TopProducts.Select(x => new ProductReportItemDto
                {
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    ProductSku = x.ProductSku,
                    TotalQuantity = x.TotalQuantity,
                    Revenue = x.Revenue,
                    Orders = x.Orders
                }).ToList()
            };

            return Result<ProductReportDto>.Success(result);
        }
    }
}
