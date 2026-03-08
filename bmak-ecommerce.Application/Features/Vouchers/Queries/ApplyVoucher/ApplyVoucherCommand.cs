using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Vouchers.Queries.ApplyVoucher
{
    public class ApplyVoucherQuery
    {
        public string Code { get; set; } = string.Empty;
        public string CartId { get; set; } = string.Empty;
    }
}
