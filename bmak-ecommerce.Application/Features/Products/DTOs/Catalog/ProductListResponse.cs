using bmak_ecommerce.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Products.DTOs.Catalog
{
    public class ProductListResponse
    {
        public PagedList<ProductSummaryDto> Products { get; set; }
        public ProductFilterAggregationDto Filters { get; set; }
    }
}
