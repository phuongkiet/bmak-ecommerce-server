using AutoMapper;
using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Orders.DTOs;
using bmak_ecommerce.Application.Features.Orders.Queries.GetAllOrders;
using bmak_ecommerce.Domain.Entities.Sales;
using bmak_ecommerce.Domain.Interfaces;
using bmak_ecommerce.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Orders.Queries.GetAllOrdersOfUser
{
    [AutoRegister]
    public class GetAllOrdersOfUserHandler : IQueryHandler<GetAllOrdersOfUserQuery, PagedList<OrderSummaryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllOrdersOfUserHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Method Handle sẽ tự động trả về Task<Result<PagedList...>> theo Interface
        public async Task<Result<PagedList<OrderSummaryDto>>> Handle(GetAllOrdersOfUserQuery request, CancellationToken cancellationToken)
        {
            if (request.Params.UserId.HasValue)
            {
                // 1. Logic lấy dữ liệu
                var query = _unitOfWork.Repository<Order>().GetAllAsQueryable();

                query = query.Where(u => u.UserId == request.Params.UserId);

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
            else
            {
                return Result<PagedList<OrderSummaryDto>>.Failure("Không tìm thấy Id người dùng.");
            }
        }
    }
}
