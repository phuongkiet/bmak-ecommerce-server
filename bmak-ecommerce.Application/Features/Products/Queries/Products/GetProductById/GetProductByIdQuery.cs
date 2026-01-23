using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Products.Queries.Products.GetProductById
{
    // 1. Định nghĩa Request (Query)
    public record GetProductByIdQuery(int Id) : IRequest<ProductDto?>;

    // 2. Định nghĩa Handler
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
    {
        private readonly IDistributedCache _cache; // Interface cache (trừu tượng)
        // private readonly IProductRepository _repository; // Interface repo của bạn

        // Inject Cache và Repository vào đây
        public GetProductByIdHandler(IDistributedCache cache /*, IProductRepository repository*/)
        {
            _cache = cache;
            // _repository = repository;
        }

        public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            string cacheKey = $"product:{request.Id}";

            // === BƯỚC 1: ĐỌC CACHE ===
            var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrEmpty(cachedData))
            {
                // Deserialize từ JSON string về ProductDto
                return JsonSerializer.Deserialize<ProductDto>(cachedData);
            }

            // === BƯỚC 2: GỌI DB (NẾU CACHE MISS) ===
            // Code giả định gọi Repository (bạn thay bằng code thật của bạn)
            // var product = await _repository.GetByIdAsync(request.Id);

            // Code giả lập để test:
            await Task.Delay(1000); // Giả vờ DB chậm
            var productDto = new ProductDto
            {
                Id = request.Id,
                Name = "Test Product from DB",
                SalePrice = 10000
            };

            if (productDto == null) return null;

            // === BƯỚC 3: LƯU CACHE (CACHE ASIDE) ===
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10), // Hết hạn sau 10p
                SlidingExpiration = TimeSpan.FromMinutes(2) // Gia hạn nếu có người xem liên tục
            };

            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(productDto),
                cacheOptions,
                cancellationToken
            );

            return productDto;
        }
    }
}
