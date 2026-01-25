using bmak_ecommerce.Application.Common.Interfaces;
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

namespace bmak_ecommerce.Application.Features.Products.Queries.Products.GetTopSellingProduct
{
    public class GetTopSellingProductsHandler : IQueryHandler<GetTopSellingProductsQuery, List<ProductSummaryDto>>
    {
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IProductRepository _productRepository;

        public GetTopSellingProductsHandler(
            IConnectionMultiplexer redisConnection,
            IProductRepository productRepository)
        {
            _redisConnection = redisConnection;
            _productRepository = productRepository;
        }

        public async Task<List<ProductSummaryDto>> Handle(GetTopSellingProductsQuery query, CancellationToken cancellationToken)
        {
            var db = _redisConnection.GetDatabase();

            // 1. Lấy Top ID từ Redis (Sorted Set)
            // Lấy theo số lượng request.Count (mặc định 10)
            var topProductIdsWithScore = await db.SortedSetRangeByRankWithScoresAsync(
                "top_selling",
                stop: query.Count - 1,
                order: StackExchange.Redis.Order.Descending
            );

            if (topProductIdsWithScore.Length == 0)
            {
                return new List<ProductSummaryDto>();
            }

            // 2. Tách List ID để query DB
            var productIds = topProductIdsWithScore
                .Select(x => int.Parse(x.Element.ToString()))
                .ToList();

            // 3. Lấy thông tin chi tiết từ MySQL
            var products = await _productRepository.GetByIdsAsync(productIds);

            // 4. Join dữ liệu để map sang DTO (Bảo toàn thứ tự từ Redis)
            var result = productIds
                .Join(products,
                      id => id,
                      p => p.Id,
                      (id, p) => new ProductSummaryDto
                      {
                          Id = p.Id,
                          Name = p.Name,
                          Slug = p.Slug,
                          Sku = p.SKU,

                          // Giá hiển thị
                          OriginalPrice = p.BasePrice,
                          Price = p.SalePrice > 0 ? p.SalePrice : p.BasePrice,

                          Thumbnail = p.Thumbnail,

                          // QUAN TRỌNG: Lấy TotalSold từ Redis Score map vào DTO
                          TotalSold = (int)topProductIdsWithScore
                                            .First(x => x.Element == id.ToString())
                                            .Score
                      })
                .ToList();

            return result;
        }
    }
}
