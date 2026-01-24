using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using bmak_ecommerce.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Products.Queries.Products.GetCatalog.Interface
{
    public interface ICatalogReadRepository
    {
        Task<ProductListResponse> GetCatalogDataAsync(ProductSpecParams specParams);
    }
}
