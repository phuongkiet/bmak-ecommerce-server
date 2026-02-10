using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Products.DTOs.Catalog
{
    public class ProductFilterAggregationDto
    {
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public List<FilterGroupDto> Categories { get; set; } = new();
        public List<FilterGroupDto> Attributes { get; set; } = new();
    }

    // Class đại diện cho 1 nhóm (Ví dụ: Nhóm Màu sắc, Nhóm Size, Nhóm Danh mục)
    public class FilterGroupDto
    {
        public string Code { get; set; } // COLOR, SIZE, CATEGORY
        public string Name { get; set; } // "Màu sắc", "Kích thước"
        public List<FilterItemDto> Options { get; set; } = new();
    }

    // Class đại diện cho 1 lựa chọn (Ví dụ: Đỏ (5 sp), Xanh (2 sp))
    public class FilterItemDto
    {
        public string Value { get; set; } // "red", "1" (id)
        public string Label { get; set; } // "Màu Đỏ", "Điện thoại"
        public int Count { get; set; }    // Số lượng sản phẩm
    }
}
