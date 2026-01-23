using bmak_ecommerce.Domain.Entities.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bmak_ecommerce.Domain.Models;

namespace bmak_ecommerce.Domain.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        // Hàm lấy danh sách sản phẩm full option (Lọc, Sort, Phân trang)
        Task<PagedList<Product>> GetProductsAsync(ProductSpecParams productParams);

        // Hàm lấy chi tiết sản phẩm kèm thuộc tính
        Task<Product?> GetProductDetailAsync(int id);
        Task<List<Product>> GetByIdsAsync(List<int> ids);
        Task<List<Product>> GetByIdsWithStocksAsync(List<int> ids);
    }
}
