using AutoMapper;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using bmak_ecommerce.Application.Features.Products.Queries.Products.GetCatalog.Interface;
using bmak_ecommerce.Domain.Interfaces;
using bmak_ecommerce.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Products.Queries.Products.GetAllProducts
{
    public class GetProductsHandler : IQueryHandler<GetProductsQuery, ProductListResponse>
    {
        // FIX: Inject Interface mới
        private readonly ICatalogReadRepository _catalogRepo;
        private readonly IMapper _mapper;

        public GetProductsHandler(ICatalogReadRepository catalogRepo, IMapper mapper)
        {
            _catalogRepo = catalogRepo;
            _mapper = mapper;
        }

        public async Task<ProductListResponse> Handle(GetProductsQuery query, CancellationToken cancellationToken)
        {
            // Gọi hàm từ Interface mới
            return await _catalogRepo.GetCatalogDataAsync(query.Params);
        }
    }
}
