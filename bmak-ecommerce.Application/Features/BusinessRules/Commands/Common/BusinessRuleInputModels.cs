using bmak_ecommerce.Domain.Enums;

namespace bmak_ecommerce.Application.Features.BusinessRules.Commands.Common
{
    public class BusinessRuleConditionInput
    {
        public string ConditionKey { get; set; } = string.Empty;
        public RuleOperator Operator { get; set; }
        public string ConditionValue { get; set; } = string.Empty;
    }

    public class BusinessRuleActionInput
    {
        public RuleActionType ActionType { get; set; }
        public decimal ActionValue { get; set; }
    }
}
