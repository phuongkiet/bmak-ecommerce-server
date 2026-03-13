namespace bmak_ecommerce.Application.Features.Categories.Admin.Commands.UpdateAdminCategory
{
    public class UpdateAdminCategoryCommand
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? ParentId { get; set; }
    }
}
