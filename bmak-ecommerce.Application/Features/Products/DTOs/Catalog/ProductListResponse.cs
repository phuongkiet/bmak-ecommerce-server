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
        // 1. Dữ liệu danh sách (đã phân trang)
        public PagedList<ProductSummaryDto> Products { get; set; }

        // 2. Dữ liệu bộ lọc (Sidebar)
        // Có thể null nếu request chỉ reload danh sách (ajax pagination)
        public ProductFilterAggregationDto? Filters { get; set; }
    }
}
