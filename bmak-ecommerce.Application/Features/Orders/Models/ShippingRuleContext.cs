namespace bmak_ecommerce.Application.Features.Orders.Models
{
    public class ShippingRuleContext
    {
        public decimal SubTotal { get; set; }
        public decimal TotalWeight { get; set; }
        public decimal TotalSquareMeter { get; set; }
        public int ItemCount { get; set; }
        public string Province { get; set; } = string.Empty;
        public string Ward { get; set; } = string.Empty;
        public string Zone { get; set; } = string.Empty;
    }
}
