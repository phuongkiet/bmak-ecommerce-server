using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using bmak_ecommerce.Application.Features.Products.Queries.Products.GetCatalog.Interface;
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
    public class ProductRepository : GenericRepository<Product>, IProductRepository, ICatalogReadRepository
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
                .Include(p => p.ProductImages)
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

            //// 3. Apply Attribute Filter (Lọc nâng cao - Phần khó nhất)
            //// Giả sử params.Attributes có dạng "size:60x60,color:grey"
            //if (!string.IsNullOrEmpty(productParams.Attributes))
            //{
            //    var attrs = productParams.Attributes.Split(','); // ["size:60x60", "color:grey"]

            //    foreach (var attr in attrs)
            //    {
            //        var parts = attr.Split(':');
            //        if (parts.Length == 2)
            //        {
            //            var code = parts[0].Trim(); // "size"
            //            var value = parts[1].Trim(); // "60x60"

            //            // Query lồng: Tìm sản phẩm CÓ chứa AttributeValue khớp Code và Value
            //            query = query.Where(p => p.AttributeValues.Any(av =>
            //                av.Attribute.Code.ToLower() == code.ToLower() &&
            //                av.Value == value));
            //        }
            //    }
            //}

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

        // Nhớ thêm từ khóa 'async' vào đầu hàm
        public async Task<ProductListResponse> GetCatalogDataAsync(ProductSpecParams specParams)
        {
            // ---------------------------------------------------------
            // PHẦN 1: TẠO QUERY CƠ SỞ
            // ---------------------------------------------------------
            var query = _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                // FIX: Tên property trong Product.cs là 'Attributes' chứ không phải 'AttributeValues'
                .Include(p => p.AttributeValues)
                    // FIX: Tên property trong ProductAttributeValue.cs là 'ProductAttribute'
                    .ThenInclude(pav => pav.Attribute)
                .AsQueryable();

            // 1. Lọc cơ bản
            if (!string.IsNullOrEmpty(specParams.Search))
                query = query.Where(p => p.Name.Contains(specParams.Search));

            if (!string.IsNullOrEmpty(specParams.CategorySlug))
                query = query.Where(p => p.Category.Slug == specParams.CategorySlug);

            // 2. Lọc theo Attributes (Dynamic)
            if (!string.IsNullOrEmpty(specParams.Color))
            {
                // FIX: pav.ProductAttribute.Code
                query = query.Where(p => p.AttributeValues.Any(pav =>
                    pav.Attribute.Code == "COLOR" && pav.Value == specParams.Color));
            }

            if (!string.IsNullOrEmpty(specParams.Size))
            {
                query = query.Where(p => p.AttributeValues.Any(pav =>
                    pav.Attribute.Code == "SIZE" && pav.Value == specParams.Size));
            }

            // 3. Lọc giá (FIX: Dùng SalePrice thay vì Price)
            if (specParams.MinPrice.HasValue)
                query = query.Where(p => p.SalePrice >= specParams.MinPrice);

            if (specParams.MaxPrice.HasValue)
                query = query.Where(p => p.SalePrice <= specParams.MaxPrice);

            // ---------------------------------------------------------
            // PHẦN 2: AGGREGATION (TÍNH TOÁN FILTER)
            // ---------------------------------------------------------
            var rawAttributes = await query
                .SelectMany(p => p.AttributeValues) // FIX: Attributes
                                               // FIX: Group theo ProductAttribute.Code
                .GroupBy(pav => new { pav.Attribute.Code, pav.Attribute.Name, pav.Value })
                .Select(g => new
                {
                    Code = g.Key.Code,
                    Label = g.Key.Name,
                    Value = g.Key.Value,
                    Count = g.Count()
                })
                .ToListAsync();

            var filtersDto = new ProductFilterAggregationDto
            {
                // FIX: Dùng SalePrice để tính min/max
                MinPrice = await query.AnyAsync() ? await query.MinAsync(p => p.SalePrice) : 0,
                MaxPrice = await query.AnyAsync() ? await query.MaxAsync(p => p.SalePrice) : 0,

                Attributes = rawAttributes
                    .GroupBy(x => x.Code)
                    .Select(g => new FilterGroupDto
                    {
                        Code = g.Key,
                        Label = g.First().Label,
                        Options = g.Select(opt => new FilterOptionDto
                        {
                            Value = opt.Value,
                            Label = opt.Value,
                            Count = opt.Count
                        }).ToList()
                    }).ToList()
            };

            // ---------------------------------------------------------
            // PHẦN 3: LẤY LIST SẢN PHẨM
            // ---------------------------------------------------------

            // Sort (FIX: Dùng SalePrice và CreatedAt)
            query = specParams.Sort switch
            {
                "priceAsc" => query.OrderBy(p => p.SalePrice),
                "priceDesc" => query.OrderByDescending(p => p.SalePrice),
                _ => query.OrderByDescending(p => p.CreatedAt) // BaseEntity dùng CreatedAt
            };

            var totalCount = await query.CountAsync();

            var products = await query
                .Skip((specParams.PageIndex - 1) * specParams.PageSize)
                .Take(specParams.PageSize)
                .Select(p => new ProductSummaryDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.SalePrice, // Map SalePrice vào Price của DTO
                    Thumbnail = p.Thumbnail, // FIX: Product.cs dùng ImageUrl
                    Slug = p.Slug,
                    Sku = p.SKU,
                    // Map thêm CategoryName nếu cần thiết
                    // CategoryName = p.Category.Name 
                })
                .ToListAsync();

            // ---------------------------------------------------------
            // PHẦN 4: RETURN
            // ---------------------------------------------------------
            return new ProductListResponse
            {
                // FIX: Property tên là PageNumber chứ không phải PageIndex (check file PagedList.cs)
                Products = new PagedList<ProductSummaryDto>(products, totalCount, specParams.PageIndex, specParams.PageSize),
                Filters = filtersDto
            };
        }

        public async Task<List<Product>> GetTopSellingProductsAsync(int count)
        {
            // Logic: Sắp xếp giảm dần theo Tổng số lượng trong OrderItems
            return await _context.Products
                .Include(p => p.OrderItems) // Load OrderItems để tính tổng
                .OrderByDescending(p => p.OrderItems.Sum(oi => oi.QuantityOnHand))
                .Take(count)
                // Include thêm ảnh/category nếu cần hiển thị ra ngoài UI
                // .Include(p => p.Category) 
                .AsNoTracking() // Read-only nên dùng AsNoTracking cho nhanh
                .ToListAsync();
        }
    }
}
