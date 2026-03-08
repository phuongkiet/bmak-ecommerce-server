using bmak_ecommerce.Domain.Enums;

namespace bmak_ecommerce.Application.Features.BusinessRules.DTOs
{
    public class BusinessRuleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Priority { get; set; }
        public bool StopProcessing { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public List<BusinessRuleConditionDto> Conditions { get; set; } = new();
        public List<BusinessRuleActionDto> Actions { get; set; } = new();
    }

    public class BusinessRuleConditionDto
    {
        public int Id { get; set; }
        public string ConditionKey { get; set; } = string.Empty;
        public RuleOperator Operator { get; set; }
        public string ConditionValue { get; set; } = string.Empty;
    }

    public class BusinessRuleActionDto
    {
        public int Id { get; set; }
        public RuleActionType ActionType { get; set; }
        public decimal ActionValue { get; set; }
    }
}
