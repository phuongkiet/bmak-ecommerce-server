using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Features.Orders.Models;
using bmak_ecommerce.Domain.Entities.Rules;
using bmak_ecommerce.Domain.Enums;
using bmak_ecommerce.Domain.Interfaces;
using System.Globalization;

namespace bmak_ecommerce.Application.Features.Orders.Services
{
    [AutoRegister]
    public class ShippingRuleEngine : IShippingRuleEngine
    {
        private readonly IBusinessRuleRepository _businessRuleRepository;

        public ShippingRuleEngine(IBusinessRuleRepository businessRuleRepository)
        {
            _businessRuleRepository = businessRuleRepository;
        }

        public async Task<ShippingRuleResult> CalculateAsync(ShippingRuleContext context, CancellationToken cancellationToken = default)
        {
            var result = new ShippingRuleResult();
            var rules = await _businessRuleRepository.GetActiveRulesAsync(cancellationToken);

            foreach (var rule in rules)
            {
                if (!IsRuleMatched(rule, context))
                {
                    continue;
                }

                result.MatchedRules.Add(rule.Name);
                ApplyActions(result, rule.Actions);

                if (rule.StopProcessing)
                {
                    break;
                }
            }

            if (result.ShippingFee < 0)
            {
                result.ShippingFee = 0;
            }

            return result;
        }

        private static bool IsRuleMatched(BusinessRule rule, ShippingRuleContext context)
        {
            if (rule.Conditions == null || rule.Conditions.Count == 0)
            {
                return true;
            }

            return rule.Conditions.All(condition => EvaluateCondition(condition, context));
        }

        private static bool EvaluateCondition(RuleCondition condition, ShippingRuleContext context)
        {
            var key = (condition.ConditionKey ?? string.Empty).Trim().ToLowerInvariant();
            var conditionValue = (condition.ConditionValue ?? string.Empty).Trim();

            return key switch
            {
                "subtotal" => CompareDecimal(context.SubTotal, condition.Operator, conditionValue),
                "totalweight" => CompareDecimal(context.TotalWeight, condition.Operator, conditionValue),
                "totalm2" => CompareDecimal(context.TotalSquareMeter, condition.Operator, conditionValue),
                "itemcount" => CompareInt(context.ItemCount, condition.Operator, conditionValue),
                "province" => CompareString(context.Province, condition.Operator, conditionValue),
                "ward" => CompareString(context.Ward, condition.Operator, conditionValue),
                "zone" => CompareString(context.Zone, condition.Operator, conditionValue),
                _ => false
            };
        }

        private static void ApplyActions(ShippingRuleResult result, IEnumerable<RuleAction> actions)
        {
            foreach (var action in actions.OrderBy(x => x.Id))
            {
                switch (action.ActionType)
                {
                    case RuleActionType.SetShippingFee:
                        result.ShippingFee = action.ActionValue;
                        break;
                    case RuleActionType.DiscountPercentage:
                        result.ShippingFee -= result.ShippingFee * (action.ActionValue / 100m);
                        break;
                    case RuleActionType.DiscountAmount:
                        result.ShippingFee -= action.ActionValue;
                        break;
                    default:
                        break;
                }
            }
        }

        private static bool CompareDecimal(decimal actualValue, RuleOperator ruleOperator, string conditionValue)
        {
            if (!decimal.TryParse(conditionValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var expectedValue)
                && !decimal.TryParse(conditionValue, NumberStyles.Any, CultureInfo.CurrentCulture, out expectedValue))
            {
                return false;
            }

            return ruleOperator switch
            {
                RuleOperator.GreaterThan => actualValue > expectedValue,
                RuleOperator.GreaterThanOrEqual => actualValue >= expectedValue,
                RuleOperator.Equal => actualValue == expectedValue,
                _ => false
            };
        }

        private static bool CompareInt(int actualValue, RuleOperator ruleOperator, string conditionValue)
        {
            if (!int.TryParse(conditionValue, out var expectedValue))
            {
                return false;
            }

            return ruleOperator switch
            {
                RuleOperator.GreaterThan => actualValue > expectedValue,
                RuleOperator.GreaterThanOrEqual => actualValue >= expectedValue,
                RuleOperator.Equal => actualValue == expectedValue,
                _ => false
            };
        }

        private static bool CompareString(string actualValue, RuleOperator ruleOperator, string conditionValue)
        {
            var normalizedActual = (actualValue ?? string.Empty).Trim();
            var normalizedExpected = (conditionValue ?? string.Empty).Trim();

            return ruleOperator switch
            {
                RuleOperator.Equal => string.Equals(normalizedActual, normalizedExpected, StringComparison.OrdinalIgnoreCase),
                RuleOperator.Contains => normalizedExpected
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Any(x => string.Equals(x, normalizedActual, StringComparison.OrdinalIgnoreCase)),
                _ => false
            };
        }
    }
}
