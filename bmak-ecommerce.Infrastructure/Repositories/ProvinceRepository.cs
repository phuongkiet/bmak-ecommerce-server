using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Domain.Entities.Catalog;
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
    [AutoRegister]

    public class ProvinceRepository : IProvinceRepository
    {
        private readonly AppDbContext _context;

        public ProvinceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedList<Province>> GetProvincesAsync(ProvinceSpecParams param)
        {
            var query = _context.Provinces.AsQueryable();

            var totalCount = await query.CountAsync();

            // Lấy dữ liệu trang hiện tại
            var items = await query
                .Skip((param.PageIndex - 1) * param.PageSize)
                .Take(param.PageSize)
                .ToListAsync();

            return new PagedList<Province>(items, totalCount, param.PageIndex, param.PageSize);
        }
    }
}
