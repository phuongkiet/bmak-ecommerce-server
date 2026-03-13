using bmak_ecommerce.Domain.Models;

namespace bmak_ecommerce.Application.Features.Categories.Admin.Queries.GetAdminCategories
{
    public class GetAdminCategoriesQuery
    {
        public CategorySpecParams Params { get; set; }

        public GetAdminCategoriesQuery(CategorySpecParams specParams)
        {
            Params = specParams;
        }
    }
}
