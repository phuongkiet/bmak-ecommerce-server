namespace bmak_ecommerce.Application.Features.Products.DTOs.Catalog
{
    public class ProductLevelDiscountDto
    {
        public int ProductId { get; set; }
        public int UserLevelId { get; set; }
        public string UserLevelCode { get; set; } = string.Empty;
        public string UserLevelName { get; set; } = string.Empty;
        public decimal DiscountPercent { get; set; }
        public bool IsActive { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
    }

    public class ProductLevelDiscountInputDto
    {
        public int UserLevelId { get; set; }
        public decimal DiscountPercent { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
    }
}
