namespace bmak_ecommerce.Domain.Models
{
    public class ProductReportResult
    {
        public int TotalProductsSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<ProductReportItem> TopProducts { get; set; } = new();
    }

    public class ProductReportItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSku { get; set; } = string.Empty;
        public int TotalQuantity { get; set; }
        public decimal Revenue { get; set; }
        public int Orders { get; set; }
    }
}
