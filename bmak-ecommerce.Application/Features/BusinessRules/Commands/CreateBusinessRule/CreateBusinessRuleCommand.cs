using bmak_ecommerce.Application.Features.BusinessRules.Commands.Common;

namespace bmak_ecommerce.Application.Features.BusinessRules.Commands.CreateBusinessRule
{
    public class CreateBusinessRuleCommand
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Priority { get; set; }
        public bool StopProcessing { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public List<BusinessRuleConditionInput> Conditions { get; set; } = new();
        public List<BusinessRuleActionInput> Actions { get; set; } = new();
    }
}
