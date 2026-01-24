using AutoMapper;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using bmak_ecommerce.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Products.Queries.Products.GetProductById
{
    public class GetProductByIdHandler : IQueryHandler<GetProductByIdQuery, ProductDto?>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public GetProductByIdHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ProductDto?> Handle(GetProductByIdQuery query, CancellationToken cancellationToken = default)
        {
            // 1. Gọi Repository lấy Entity (đã Include đủ thứ)
            var product = await _productRepository.GetProductDetailAsync(query.Id);

            if (product == null) return null;

            // 2. Map Entity -> ProductDto
            // (Bạn có thể dùng AutoMapper hoặc map tay như dưới đây cho tường minh)
            var dto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,
                Sku = product.SKU,
                SpecificationsJson = product.SpecificationsJson,
                Price = product.SalePrice,
                OriginalPrice = product.BasePrice,
                //StockQuantity = product.Stocks.Select(s => s.QuantityOnHand),
                Thumbnail = product.Thumbnail,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? string.Empty,

                // Map Images
                Images = product.ProductImages.Select(img => new ProductImageDto
                {
                    Id = img.Id,
                    Url = img.ImageUrl,
                    IsMain = img.IsMain,
                    SortOrder = img.SortOrder
                }).OrderBy(i => i.SortOrder).ToList(),

                // Map Attributes
                Attributes = product.AttributeValues.Select(attr => new ProductAttributeDto
                {
                    Code = attr.Attribute.Code,
                    Name = attr.Attribute.Name,
                    Value = attr.Value
                }).ToList()
            };

            return dto;
        }
    }
}
