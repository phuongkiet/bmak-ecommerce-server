using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Domain.Entities.Sales;
using bmak_ecommerce.Domain.Interfaces;
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
    public class VoucherRepository : IVoucherRepository
    {
        private readonly AppDbContext _context;

        public VoucherRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Voucher?> GetByIdAsync(int id)
        {
            return await _context.Vouchers.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task<Voucher?> GetByCodeAsync(string code)
        {
            var normalizedCode = code.Trim().ToUpperInvariant();

            return await _context.Vouchers
                .FirstOrDefaultAsync(x => x.Code == normalizedCode && !x.IsDeleted);
        }

        public async Task<bool> IsCodeExistsAsync(string code, int? excludeId = null)
        {
            var normalizedCode = code.Trim().ToUpperInvariant();

            return await _context.Vouchers.AnyAsync(x =>
                !x.IsDeleted
                && x.Code == normalizedCode
                && (!excludeId.HasValue || x.Id != excludeId.Value));
        }
    }
}
