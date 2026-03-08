namespace bmak_ecommerce.Application.Features.Vouchers.Queries.GetVouchers
{
    public class GetVouchersQuery
    {
        public VoucherSpecParams Params { get; }

        public GetVouchersQuery(VoucherSpecParams @params)
        {
            Params = @params;
        }
    }
}
