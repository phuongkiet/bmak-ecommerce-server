using bmak_ecommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Entities.Rules
{
    public class BusinessRule : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; } // 1, 2, 3...
        public bool StopProcessing { get; set; } // Chạy xong dừng luôn

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<RuleCondition> Conditions { get; set; } = new List<RuleCondition>();
        public virtual ICollection<RuleAction> Actions { get; set; } = new List<RuleAction>();
    }
}
