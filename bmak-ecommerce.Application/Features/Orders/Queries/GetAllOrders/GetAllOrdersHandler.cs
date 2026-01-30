using AutoMapper;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Products.DTOs.Sale;
using bmak_ecommerce.Domain.Entities.Sales;
using bmak_ecommerce.Domain.Interfaces;
using bmak_ecommerce.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace bmak_ecommerce.Application.Features.Orders.Queries.GetAllOrders
{
    public class GetOrdersHandler : IQueryHandler<GetOrdersQuery, PagedList<OrderSummaryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetOrdersHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Method Handle sẽ tự động trả về Task<Result<PagedList...>> theo Interface
        public async Task<Result<PagedList<OrderSummaryDto>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            // 1. Logic lấy dữ liệu
            var query = _unitOfWork.Repository<Order>().GetAllAsQueryable();

            // ... Logic Filter / Search ...

            // 2. Query DB với pagination
            var totalCount = await query.CountAsync(cancellationToken);
            var orders = await query
                .Skip((request.Params.PageIndex - 1) * request.Params.PageSize)
                .Take(request.Params.PageSize)
                .ToListAsync(cancellationToken);

            // 3. Map sang DTO
            var orderDtos = _mapper.Map<List<OrderSummaryDto>>(orders);

            var pagedListDto = new PagedList<OrderSummaryDto>(
                orderDtos,
                totalCount,
                request.Params.PageIndex,
                request.Params.PageSize
            );

            // 4. Trả về Result.Success
            return Result<PagedList<OrderSummaryDto>>.Success(pagedListDto);
        }
    }
}
