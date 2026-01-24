using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Products.DTOs.Catalog
{
    public class ProductFilterAggregationDto
    {
        // Thống kê giá thấp nhất/cao nhất trong tập kết quả
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }

        // Các nhóm thuộc tính (Màu sắc, Kích thước...)
        public List<FilterGroupDto> Attributes { get; set; } = new();
    }

    public class FilterGroupDto
    {
        public string Code { get; set; } // VD: COLOR
        public string Label { get; set; } // VD: Màu sắc
        public List<FilterOptionDto> Options { get; set; } = new();
    }

    public class FilterOptionDto
    {
        public string Value { get; set; } // VD: Red
        public string Label { get; set; } // VD: Đỏ
        public int Count { get; set; }    // Số lượng sản phẩm (quan trọng cho dynamic filter)
    }
}
