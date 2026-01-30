using AutoMapper;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Domain.Models;
using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace bmak_ecommerce.Application.Features.Products.Queries.Products.GetAllProducts
{
    public class GetProductsHandler : IQueryHandler<GetProductsQuery, ProductListResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetProductsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<ProductListResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var specParams = request.Params;

            // 1. Lấy Query
            var query = _unitOfWork.Repository<Product>().GetAllAsQueryable();

            // ... (Đoạn code filter/sort giữ nguyên) ...
            if (!string.IsNullOrEmpty(specParams.Search))
            {
                var search = specParams.Search.ToLower();
                query = query.Where(x => x.Name.ToLower().Contains(search));
            }
            // ...

            // 2. Query DB với pagination
            var totalCount = await query.CountAsync(cancellationToken);
            var products = await query
                .Skip((specParams.PageIndex - 1) * specParams.PageSize)
                .Take(specParams.PageSize)
                .ToListAsync(cancellationToken);

            // 3. MAP DATA sang DTO
            var dtos = _mapper.Map<List<ProductSummaryDto>>(products);

            // 4. Tạo PagedList với đầy đủ 4 tham số
            var pagedListDto = new PagedList<ProductSummaryDto>(
                dtos,
                totalCount,
                specParams.PageIndex,
                specParams.PageSize
            );

            // 4. Tạo ProductListResponse
            var response = new ProductListResponse
            {
                Products = pagedListDto,
                Filters = new ProductFilterAggregationDto // Empty filters for now
                {
                    MinPrice = 0,
                    MaxPrice = 0,
                    Attributes = new List<FilterGroupDto>()
                }
            };

            // 3. Trả về kết quả
            return Result<ProductListResponse>.Success(response);
        }
    }
}
