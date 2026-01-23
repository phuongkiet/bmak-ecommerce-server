using AutoMapper;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Products.DTOs.Sale;
using bmak_ecommerce.Domain.Interfaces;
using bmak_ecommerce.Domain.Models;

namespace bmak_ecommerce.Application.Features.Products.Queries.Orders.GetAllOrders
{
    public class GetOrdersHandler : IQueryHandler<GetOrdersQuery, Result<PagedList<OrderSummaryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetOrdersHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // 4. Thay đổi Return Type của hàm Handle
        public async Task<Result<PagedList<OrderSummaryDto>>> Handle(GetOrdersQuery query, CancellationToken cancellationToken = default)
        {
            try
            {
                // 1. Gọi Repository
                var pagedEntities = await _unitOfWork.Orders.GetOrdersWithPaginationQuery(query.Params);

                // 2. Map Entity -> DTO
                var dtos = _mapper.Map<List<OrderSummaryDto>>(pagedEntities.Items);

                // 3. Tạo PagedList DTO
                var pagedListDto = new PagedList<OrderSummaryDto>(
                    dtos,
                    pagedEntities.TotalCount,
                    pagedEntities.PageIndex,
                    query.Params.PageSize
                );

                // 5. WRAP KẾT QUẢ VÀO RESULT.SUCCESS
                return Result<PagedList<OrderSummaryDto>>.Success(pagedListDto);
            }
            catch (Exception ex)
            {
                return Result<PagedList<OrderSummaryDto>>.Failure($"Lỗi khi lấy danh sách đơn hàng: {ex.Message}");
            }
        }
    }
}
