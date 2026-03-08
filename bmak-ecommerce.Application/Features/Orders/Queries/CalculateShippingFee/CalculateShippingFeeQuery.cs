namespace bmak_ecommerce.Application.Features.Orders.Queries.CalculateShippingFee
{
    public class CalculateShippingFeeQuery
    {
        public string CartId { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string Ward { get; set; } = string.Empty;
    }
}
