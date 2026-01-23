using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Enums
{
    public enum RuleActionType
    {
        SetShippingFee = 1,     // Gán phí ship bằng X
        DiscountPercentage = 2, // Giảm X%
        DiscountAmount = 3,     // Giảm X tiền
        FreeGift = 4            // Tặng quà
    }
}
