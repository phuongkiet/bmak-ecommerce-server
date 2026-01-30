using AutoMapper;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Domain.Models;
using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using bmak_ecommerce.Domain.Entities.Sales;
using bmak_ecommerce.Domain.Interfaces;
using MediatR;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bmak_ecommerce.Application.Common.Models;

namespace bmak_ecommerce.Application.Features.Products.Queries.Products.GetTopSellingProduct
{
    public class GetTopSellingHandler : IQueryHandler<GetTopSellingProductsQuery, List<ProductSummaryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetTopSellingHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<ProductSummaryDto>>> Handle(GetTopSellingProductsQuery request, CancellationToken cancellationToken)
        {
            // 1. Gọi Repo lấy Entity
            var products = await _unitOfWork.Products.GetTopSellingProductsAsync(request.Count);

            // 2. Map sang DTO rút gọn
            var dtos = _mapper.Map<List<ProductSummaryDto>>(products);

            // 3. Trả về Result
            return Result<List<ProductSummaryDto>>.Success(dtos);
        }
    }
}
