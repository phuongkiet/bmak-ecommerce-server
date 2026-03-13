using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;

namespace bmak_ecommerce.Application.Features.Products.Commands.UpsertProductLevelDiscounts
{
    public class UpsertProductLevelDiscountsCommand
    {
        public int ProductId { get; set; }
        public List<ProductLevelDiscountInputDto> Discounts { get; set; } = new();
    }
}
