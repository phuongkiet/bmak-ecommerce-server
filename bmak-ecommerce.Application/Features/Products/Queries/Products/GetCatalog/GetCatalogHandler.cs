using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Domain.Models;
using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using bmak_ecommerce.Application.Features.Products.Queries.Products.GetCatalog.Interface;
using bmak_ecommerce.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Common.Attributes;

namespace bmak_ecommerce.Application.Features.Products.Queries.Products.GetCatalog
{
    [AutoRegister]

    public class GetCatalogHandler : IQueryHandler<GetCatalogQuery, ProductListResponse>
    {
        // Inject Interface Repository (chuyên đọc dữ liệu)
        private readonly ICatalogReadRepository _catalogReadRepository;

        public GetCatalogHandler(ICatalogReadRepository catalogReadRepository)
        {
            _catalogReadRepository = catalogReadRepository;
        }

        // --- SỬA Ở ĐÂY ---
        // 1. Return Type: Phải là Task<Result<ProductListResponse>>
        public async Task<Result<ProductListResponse>> Handle(GetCatalogQuery query, CancellationToken cancellationToken)
        {
            // 2. Lấy dữ liệu
            var data = await _catalogReadRepository.GetCatalogDataAsync(query.Params);

            // 3. Wrap vào Result.Success
            return Result<ProductListResponse>.Success(data);
        }
    }
}
