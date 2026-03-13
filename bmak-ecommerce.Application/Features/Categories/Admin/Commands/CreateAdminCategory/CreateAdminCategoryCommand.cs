namespace bmak_ecommerce.Application.Features.Categories.Admin.Commands.CreateAdminCategory
{
    public class CreateAdminCategoryCommand
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? ParentId { get; set; }
    }
}
