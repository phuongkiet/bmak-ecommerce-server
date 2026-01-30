using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using bmak_ecommerce.Application.Features.Products.DTOs.Sale;
using bmak_ecommerce.Domain.Models;

namespace bmak_ecommerce.Application.Features.Orders.Queries.GetAllOrders
{
    public class GetOrdersQuery
    {
        public OrderSpecParams Params { get; }

        public GetOrdersQuery(OrderSpecParams specParams)
        {
            Params = specParams;
        }
    }
}
