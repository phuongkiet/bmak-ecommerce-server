using bmak_ecommerce.Domain.Entities.Directory;
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
    public class WardRepository : IWardRepository
    {
        private readonly AppDbContext _context;

        public WardRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedList<Ward>> GetWardsByProvinceAsync(WardSpecParams param)
        {
            var query = _context.Wards.Where(w => w.ProvinceId == param.ProvinceId).AsQueryable();

            var totalCount = await query.CountAsync();

            // Lấy dữ liệu trang hiện tại
            var items = await query
                .Skip((param.PageIndex - 1) * param.PageSize)
                .Take(param.PageSize)
                .ToListAsync();

            return new PagedList<Ward>(items, totalCount, param.PageIndex, param.PageSize);
        }
    }
}
