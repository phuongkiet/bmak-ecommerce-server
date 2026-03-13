namespace bmak_ecommerce.Application.Features.Categories.Admin.Commands.DeleteAdminCategory
{
    public class DeleteAdminCategoryCommand
    {
        public int Id { get; set; }

        public DeleteAdminCategoryCommand(int id)
        {
            Id = id;
        }
    }
}
