using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Interfaces;
using bmak_ecommerce.Domain.Models;
using bmak_ecommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Product?> GetProductDetailAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.AttributeValues)
                    .ThenInclude(av => av.Attribute) // Include sâu vào tên thuộc tính
                .Include(p => p.ProductTags)
                    .ThenInclude(pt => pt.Tag)
                .Include(p => p.TierPrices)
                .Include(p => p.Stocks)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Product>> GetByIdsWithStocksAsync(List<int> ids)
        {
            return await _context.Products
                .Include(p => p.Stocks) // BẮT BUỘC
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();
        }

        public async Task<PagedList<Product>> GetProductsAsync(ProductSpecParams productParams)
        {
            // 1. Khởi tạo Query (Chưa chạy xuống DB)
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.AttributeValues)
                    .ThenInclude(av => av.Attribute)
                .Include(p => p.ProductTags)
                    .ThenInclude(pt => pt.Tag)
                .AsQueryable();

            // 2. Apply Filters (Lọc cơ bản)

            // Tìm kiếm theo tên
            if (!string.IsNullOrEmpty(productParams.Search))
            {
                var search = productParams.Search.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(search));
            }

            // Lọc theo danh mục
            if (productParams.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == productParams.CategoryId);
            }

            // Lọc theo khoảng giá
            if (productParams.MinPrice.HasValue)
                query = query.Where(p => p.SalePrice >= productParams.MinPrice);

            if (productParams.MaxPrice.HasValue)
                query = query.Where(p => p.SalePrice <= productParams.MaxPrice);

            // 3. Apply Attribute Filter (Lọc nâng cao - Phần khó nhất)
            // Giả sử params.Attributes có dạng "size:60x60,color:grey"
            if (!string.IsNullOrEmpty(productParams.Attributes))
            {
                var attrs = productParams.Attributes.Split(','); // ["size:60x60", "color:grey"]

                foreach (var attr in attrs)
                {
                    var parts = attr.Split(':');
                    if (parts.Length == 2)
                    {
                        var code = parts[0].Trim(); // "size"
                        var value = parts[1].Trim(); // "60x60"

                        // Query lồng: Tìm sản phẩm CÓ chứa AttributeValue khớp Code và Value
                        query = query.Where(p => p.AttributeValues.Any(av =>
                            av.Attribute.Code.ToLower() == code.ToLower() &&
                            av.Value == value));
                    }
                }
            }

            // 4. Filter by Tags (Lọc theo Tag IDs)
            // Nếu có nhiều TagIds, sẽ filter products có TẤT CẢ các tags đó (AND logic)
            if (productParams.TagIds != null && productParams.TagIds.Any())
            {
                foreach (var tagId in productParams.TagIds)
                {
                    query = query.Where(p => p.ProductTags.Any(pt => pt.TagId == tagId));
                }
            }

            // 5. Sorting (Sắp xếp)
            query = productParams.Sort switch
            {
                "priceAsc" => query.OrderBy(p => p.SalePrice),
                "priceDesc" => query.OrderByDescending(p => p.SalePrice),
                "nameAsc" => query.OrderBy(p => p.Name),
                _ => query.OrderByDescending(p => p.CreatedAt) // Mặc định: Mới nhất lên đầu
            };

            // 6. Pagination (Phân trang)
            // Đếm tổng số record thỏa mãn điều kiện TRƯỚC khi phân trang
            var totalCount = await query.CountAsync();

            // Lấy dữ liệu trang hiện tại
            var items = await query
                .Skip((productParams.PageIndex - 1) * productParams.PageSize)
                .Take(productParams.PageSize)
                .ToListAsync();

            return new PagedList<Product>(items, totalCount, productParams.PageIndex, productParams.PageSize);
        }

        public async Task<List<Product>> GetByIdsAsync(List<int> ids)
        {
            // Lệnh này tương đương: SELECT * FROM Products WHERE Id IN (1, 5, 9...)
            return await _context.Products
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();
        }
    }
}
