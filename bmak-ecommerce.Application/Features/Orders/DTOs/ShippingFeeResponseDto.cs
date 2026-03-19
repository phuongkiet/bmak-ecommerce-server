namespace bmak_ecommerce.Application.Features.Orders.DTOs
{
    public class ShippingFeeResponseDto
    {
        public decimal ShippingFee { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalWeight { get; set; }
        public decimal TotalSquareMeter { get; set; }
        public string ResolvedZone { get; set; } = string.Empty;
        public List<string> MatchedRules { get; set; } = new();
    }
}
