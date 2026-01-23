using bmak_ecommerce.Domain.Common;
using bmak_ecommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Entities.Rules
{
    public class RuleCondition : BaseEntity
    {
        public string ConditionKey { get; set; } // VD: "TotalM2", "City"
        public RuleOperator Operator { get; set; } // >, =, contains
        public string ConditionValue { get; set; } // "30", "HCM"

        public int BusinessRuleId { get; set; }
        public virtual BusinessRule BusinessRule { get; set; }
    }
}
