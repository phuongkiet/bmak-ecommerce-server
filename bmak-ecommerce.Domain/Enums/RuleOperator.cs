using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Enums
{
    public enum RuleOperator
    {
        GreaterThan = 1,    // >
        GreaterThanOrEqual = 2, // >=
        Equal = 3,          // =
        Contains = 4        // Trong danh sách (IN)
    }
}
