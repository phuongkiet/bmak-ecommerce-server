namespace bmak_ecommerce.Application.Features.Categories.Admin.Queries.GetAdminCategoryDetail
{
    public class GetAdminCategoryDetailQuery
    {
        public int Id { get; set; }

        public GetAdminCategoryDetailQuery(int id)
        {
            Id = id;
        }
    }
}
