namespace bmak_ecommerce.Application.Features.Vouchers.Queries.GetVoucherDetail
{
    public class GetVoucherDetailQuery
    {
        public int Id { get; set; }

        public GetVoucherDetailQuery(int id)
        {
            Id = id;
        }
    }
}
