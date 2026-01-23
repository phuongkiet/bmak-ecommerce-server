using bmak_ecommerce.Domain.Common;
using bmak_ecommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Entities.Rules
{
    public class RuleAction : BaseEntity
    {
        public RuleActionType ActionType { get; set; } // FreeShip, Discount...
        public decimal ActionValue { get; set; }

        public int BusinessRuleId { get; set; }
        public virtual BusinessRule BusinessRule { get; set; }
    }
}
