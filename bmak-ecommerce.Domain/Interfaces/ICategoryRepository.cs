using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Models;

namespace bmak_ecommerce.Domain.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        // Hàm lấy danh sách danh mục với filter và phân trang
        Task<PagedList<Category>> GetCategoriesAsync(CategorySpecParams categoryParams);

        // Hàm lấy chi tiết danh mục kèm parent và products
        Task<Category?> GetCategoryDetailAsync(int id);
    }
}



