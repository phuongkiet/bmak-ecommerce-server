namespace bmak_ecommerce.Application.Features.Reports.Queries.GetProductReport
{
    public class GetProductReportQuery
    {
        public DateTime? FromDate { get; }
        public DateTime? ToDate { get; }
        public int Top { get; }

        public GetProductReportQuery(DateTime? fromDate, DateTime? toDate, int top)
        {
            FromDate = fromDate;
            ToDate = toDate;
            Top = top;
        }
    }
}
