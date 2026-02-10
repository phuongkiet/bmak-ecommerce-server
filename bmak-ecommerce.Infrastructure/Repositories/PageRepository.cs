using bmak_ecommerce.Domain.Models;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using bmak_ecommerce.Domain.Interfaces;
using bmak_ecommerce.Domain.Entities.Page;
using bmak_ecommerce.Application.Common.Attributes;

namespace bmak_ecommerce.Infrastructure.Repositories
{
    [AutoRegister]

    public class PageRepository : GenericRepository<Page>, IPageRepository
    {
        public PageRepository(AppDbContext context) : base(context) { }

        public void Add(Page page)
        {
            _context.Pages.Add(page);
        }

        public void Update(Page page)
        {
            // Trong EF Core, nếu Entity đã được load lên và thay đổi thuộc tính, 
            // nó sẽ tự động được đánh dấu là Modified. 
            // Tuy nhiên, lệnh này đảm bảo trạng thái được ghi nhận chính xác.
            _context.Pages.Update(page);
        }

        public void Delete(Page page)
        {
            _context.Pages.Remove(page);
        }

        public async Task<Page?> GetPageByIdAsync(int id)
        {
            return await _context.Pages.FindAsync(id);
        }
        public async Task<Page?> GetBySlugAsync(string slug)
        {
            return await _context.Pages
                .FirstOrDefaultAsync(x => x.Slug == slug && !x.IsDeleted);
        }

        public async Task<Page?> GetPageDetailAsync(string slug)
        {
            return await _context.Pages.FirstOrDefaultAsync(c => c.Slug == slug);
        }

        public async Task<PagedList<Page>> GetPagesAsync(PageSpecParams pageParams)
        {
            var query = _context.Pages.AsQueryable();

            // Tìm kiếm theo tên
            if (!string.IsNullOrEmpty(pageParams.Search))
            {
                var search = pageParams.Search.ToLower();
                query = query.Where(c => c.Title.ToLower().Contains(search) && c.Slug.ToLower().Contains(search));
            }

            // 3. Apply Sorting
            query = pageParams.Sort?.ToLower() switch
            {
                "namedesc" => query.OrderByDescending(c => c.Title),
                "nameasc" => query.OrderBy(c => c.Title),
                _ => query.OrderBy(c => c.Title) // Mặc định sắp xếp theo tên
            };

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageParams.PageIndex - 1) * pageParams.PageSize)
                .Take(pageParams.PageSize)
                .ToListAsync();

            return new PagedList<Page>(
                items,
                totalCount,
                pageParams.PageIndex,
                pageParams.PageSize
            );
        }
    }
}



