namespace bmak_ecommerce.Application.Features.NewsCategories.Queries
{
    public class GetNewsCategoryDetailQuery
    {
        public int Id { get; set; }

        public GetNewsCategoryDetailQuery(int id)
        {
            Id = id;
        }
    }
}
