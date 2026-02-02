using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Entities.Page;
using bmak_ecommerce.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Interfaces
{
    public interface IPageRepository : IGenericRepository<Page>
    {
        Task<PagedList<Page>> GetPagesAsync(PageSpecParams pageParams);
        Task<Page?> GetPageDetailAsync(string slug);
        Task<Page?> GetPageByIdAsync(int id);
        Task<Page?> GetBySlugAsync(string slug);
        void Add(Page page);
        void Delete(Page page);
    }
}
