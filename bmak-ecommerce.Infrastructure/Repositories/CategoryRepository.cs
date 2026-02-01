using bmak_ecommerce.Domain.Models;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Interfaces;
using bmak_ecommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace bmak_ecommerce.Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Category?> GetCategoryDetailAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.Parent)
                .Include(c => c.Children)
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Domain.Models.PagedList<Category>> GetCategoriesAsync(CategorySpecParams categoryParams)
        {
            // 1. Khởi tạo Query (Chưa chạy xuống DB)
            var query = _context.Categories
                .Include(c => c.Parent)
                .Include(c => c.Products)
                .AsQueryable();

            // 2. Apply Filters

            // Tìm kiếm theo tên
            if (!string.IsNullOrEmpty(categoryParams.Search))
            {
                var search = categoryParams.Search.ToLower();
                query = query.Where(c => c.Name.ToLower().Contains(search));
            }

            // Lọc theo danh mục cha
            if (categoryParams.ParentId.HasValue)
            {
                query = query.Where(c => c.ParentId == categoryParams.ParentId.Value);
            }
            else if (categoryParams.IncludeOnlyRootCategories == true)
            {
                // Chỉ lấy danh mục gốc (không có parent)
                query = query.Where(c => c.ParentId == null);
            }

            // 3. Apply Sorting
            query = categoryParams.Sort?.ToLower() switch
            {
                "namedesc" => query.OrderByDescending(c => c.Name),
                "nameasc" => query.OrderBy(c => c.Name),
                "productcountdesc" => query.OrderByDescending(c => c.Products.Count),
                _ => query.OrderBy(c => c.Name) // Mặc định sắp xếp theo tên
            };

            // 4. Lấy tổng số bản ghi (trước khi phân trang)
            var totalCount = await query.CountAsync();

            // 5. Apply Pagination
            var items = await query
                .Skip((categoryParams.PageIndex - 1) * categoryParams.PageSize)
                .Take(categoryParams.PageSize)
                .ToListAsync();

            // 6. Trả về PagedList
            return new Domain.Models.PagedList<Category>(
                items,
                totalCount,
                categoryParams.PageIndex,
                categoryParams.PageSize
            );
        }
    }
}



