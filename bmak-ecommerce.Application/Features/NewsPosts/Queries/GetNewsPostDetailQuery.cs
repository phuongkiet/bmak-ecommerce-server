namespace bmak_ecommerce.Application.Features.NewsPosts.Queries
{
    public class GetNewsPostDetailQuery
    {
        public int Id { get; set; }

        public GetNewsPostDetailQuery(int id)
        {
            Id = id;
        }
    }
}
