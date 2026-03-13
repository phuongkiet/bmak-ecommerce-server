using AutoMapper;
using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Orders.DTOs;
using bmak_ecommerce.Domain.Interfaces;
using bmak_ecommerce.Domain.Models;

namespace bmak_ecommerce.Application.Features.Orders.Queries.GetAllOrders
{
    [AutoRegister]

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
            var pagedOrders = await _unitOfWork.Orders.GetOrdersWithPaginationQuery(request.Params);

            var orderDtos = _mapper.Map<List<OrderSummaryDto>>(pagedOrders.Items);

            var pagedListDto = new PagedList<OrderSummaryDto>(
                orderDtos,
                pagedOrders.TotalCount,
                pagedOrders.PageIndex,
                pagedOrders.PageSize
            );

            return Result<PagedList<OrderSummaryDto>>.Success(pagedListDto);
        }
    }
}
