using bmak_ecommerce.Application.Features.Orders.Models;

namespace bmak_ecommerce.Application.Common.Interfaces
{
    public interface IShippingRuleEngine
    {
        Task<ShippingRuleResult> CalculateAsync(ShippingRuleContext context, CancellationToken cancellationToken = default);
    }
}
