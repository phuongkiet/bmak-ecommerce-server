using AutoMapper;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
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
    public class GetProductsHandler : IQueryHandler<GetProductsQuery, PagedList<ProductDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public GetProductsHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<PagedList<ProductDto>> Handle(GetProductsQuery query, CancellationToken cancellationToken = default)
        {
            // Logic giữ nguyên, chỉ thay đổi input là query.Params
            var pagedEntities = await _productRepository.GetProductsAsync(query.Params);

            var dtos = _mapper.Map<List<ProductDto>>(pagedEntities.Items);

            return new PagedList<ProductDto>(
                dtos,
                pagedEntities.TotalCount,
                pagedEntities.PageIndex,
                query.Params.PageSize
            );
        }
    }
}
