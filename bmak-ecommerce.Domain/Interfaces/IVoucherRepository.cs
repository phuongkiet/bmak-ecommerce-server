using bmak_ecommerce.Domain.Entities.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Interfaces
{
    public interface IVoucherRepository
    {
        Task<Voucher?> GetByIdAsync(int id);
        Task<Voucher?> GetByCodeAsync(string code);
        Task<bool> IsCodeExistsAsync(string code, int? excludeId = null);
    }
}
