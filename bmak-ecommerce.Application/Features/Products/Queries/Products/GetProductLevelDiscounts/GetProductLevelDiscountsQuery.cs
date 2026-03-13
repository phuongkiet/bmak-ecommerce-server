using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;

namespace bmak_ecommerce.Application.Features.Products.Queries.Products.GetProductLevelDiscounts
{
    public class GetProductLevelDiscountsQuery
    {
        public int ProductId { get; set; }

        public GetProductLevelDiscountsQuery(int productId)
        {
            ProductId = productId;
        }
    }
}
