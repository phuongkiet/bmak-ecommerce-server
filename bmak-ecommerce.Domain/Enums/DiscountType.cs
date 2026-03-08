using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Enums
{
    public enum DiscountType
    {
        FixedAmount = 1, // Giảm số tiền cố định (VD: Giảm 50.000đ)
        Percentage = 2   // Giảm theo phần trăm (VD: Giảm 10%)
    }
}
