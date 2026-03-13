namespace bmak_ecommerce.Application.Features.Reports.DTOs
{
    public class RevenueReportDto
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<RevenueByDateDto> RevenueByDate { get; set; } = new();
    }

    public class RevenueByDateDto
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public int Orders { get; set; }
    }
}
