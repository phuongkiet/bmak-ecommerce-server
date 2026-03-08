using AutoMapper;
using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Orders.DTOs;
using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using bmak_ecommerce.Application.Features.Products.Queries.Products.GetProductById;
using bmak_ecommerce.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Orders.Queries.GetOrderById
{
    [AutoRegister]
    public class GetOrderByIdHandler : IQueryHandler<GetOrderByIdQuery, OrderDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetOrderByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<OrderDto?>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            // 1. Gọi hàm Repository chuyên dụng đã viết ở Bước 1
            var order = await _unitOfWork.Orders.GetOrderDetailAsync(request.OrderCode);

            // 2. Kiểm tra Null -> Trả về Failure
            if (order == null)
            {
                return Result<OrderDto?>.Failure($"Không tìm thấy sản phẩm có ID = {request.OrderCode}");
            }

            // 3. Map Entity -> DTO
            var orderDto = _mapper.Map<OrderDto>(order);

            // 5. Trả về Success
            return Result<OrderDto?>.Success(orderDto);
        }
    }
}
