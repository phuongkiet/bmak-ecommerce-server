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
    public class GetTopSellingProductsHandler : IRequestHandler<GetTopSellingProductsQuery, List<ProductDto>>
    {
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IProductRepository _productRepository; 

        public GetTopSellingProductsHandler(IConnectionMultiplexer redisConnection, IProductRepository productRepository)
        {
            _redisConnection = redisConnection;
            _productRepository = productRepository;
        }

        public async Task<List<ProductDto>> Handle(GetTopSellingProductsQuery request, CancellationToken cancellationToken)
        {
            var db = _redisConnection.GetDatabase();

            // 1. Lấy Top 10 ID từ Redis
            // Order.Descending: Sắp xếp giảm dần (cao nhất đứng đầu)
            // Take: 10
            var topProductIdsWithScore = await db.SortedSetRangeByRankWithScoresAsync("top_selling", stop: 9, order: StackExchange.Redis.Order.Descending);

            if (topProductIdsWithScore.Length == 0)
            {
                return new List<ProductDto>();
            }

            // 2. Trích xuất list ID ra
            var productIds = topProductIdsWithScore
                .Select(x => int.Parse(x.Element.ToString()))
                .ToList();

            // 3. Lấy thông tin chi tiết (Tên, Giá...) từ Database dựa trên List ID
            // Bạn nên viết hàm GetByIdsAsync trong Repository (WHERE Id IN (...))
            var products = await _productRepository.GetByIdsAsync(productIds);

            // 4. Sắp xếp lại list products theo đúng thứ tự của Redis 
            // (Vì SQL query WHERE IN thường trả về thứ tự lung tung)
            var result = productIds
                .Join(products,
                      id => id,
                      p => p.Id,
                      (id, p) => new ProductDto
                      {
                          Id = p.Id,
                          Name = p.Name,
                          BasePrice = p.BasePrice,
                          SalePrice = p.SalePrice,
                          // Mẹo: Bạn có thể lấy luôn số lượng đã bán từ Redis gán vào đây
                          TotalSold = (int)topProductIdsWithScore.First(x => x.Element == id.ToString()).Score
                      })
                .ToList();

            return result;
        }
    }
}
