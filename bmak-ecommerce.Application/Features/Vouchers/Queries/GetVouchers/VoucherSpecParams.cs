namespace bmak_ecommerce.Application.Features.Vouchers.Queries.GetVouchers
{
    public class VoucherSpecParams
    {
        private const int MaxPageSize = 100;

        public int PageIndex { get; set; } = 1;

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
