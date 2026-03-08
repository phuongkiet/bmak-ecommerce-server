namespace bmak_ecommerce.Application.Features.Orders.Models
{
    public class ShippingRuleResult
    {
        public decimal ShippingFee { get; set; }
        public List<string> MatchedRules { get; set; } = new();
    }
}
