using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using bmak_ecommerce.Application.Features.Products.Queries.Products.GetCatalog.Interface;
using bmak_ecommerce.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Products.Queries.Products.GetCatalog
{
    public class GetCatalogHandler : IQueryHandler<GetCatalogQuery, ProductListResponse>
    {
        // Inject Interface mới
        private readonly ICatalogReadRepository _catalogReadRepository;

        public GetCatalogHandler(ICatalogReadRepository catalogReadRepository)
        {
            _catalogReadRepository = catalogReadRepository;
        }

        public async Task<ProductListResponse> Handle(GetCatalogQuery query, CancellationToken cancellationToken)
        {
            // Gọi hàm từ interface mới
            return await _catalogReadRepository.GetCatalogDataAsync(query.Params);
        }
    }
}
