namespace bmak_ecommerce.Application.Features.Reports.Queries.GetRevenueReport
{
    public class GetRevenueReportQuery
    {
        public DateTime? FromDate { get; }
        public DateTime? ToDate { get; }

        public GetRevenueReportQuery(DateTime? fromDate, DateTime? toDate)
        {
            FromDate = fromDate;
            ToDate = toDate;
        }
    }
}
