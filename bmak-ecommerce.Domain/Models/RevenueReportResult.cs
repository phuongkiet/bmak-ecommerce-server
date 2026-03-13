namespace bmak_ecommerce.Domain.Models
{
    public class RevenueReportResult
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<RevenueByDateItem> RevenueByDate { get; set; } = new();
    }

    public class RevenueByDateItem
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public int Orders { get; set; }
    }
}
