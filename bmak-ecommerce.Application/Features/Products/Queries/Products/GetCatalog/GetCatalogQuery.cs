using bmak_ecommerce.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Products.Queries.Products.GetCatalog
{
    public class GetCatalogQuery
    {
        public ProductSpecParams Params { get; }
        public GetCatalogQuery(ProductSpecParams specParams) => Params = specParams;
    }
}
