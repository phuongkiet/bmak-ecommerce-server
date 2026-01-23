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
        public List<FilterGroupDto> Brands { get; set; } = new();
        public List<FilterGroupDto> Attributes { get; set; } = new(); // Màu sắc, Kích thước...
    }

    // Nhóm bộ lọc (Ví dụ: Nhóm "Màu sắc")
    public class FilterGroupDto
    {
        public string Label { get; set; } = string.Empty; // Tên hiển thị: "Màu sắc"
        public string Code { get; set; } = string.Empty;  // Key để gửi lên API: "color"
        public List<FilterOptionDto> Options { get; set; } = new();
    }

    // Tùy chọn con (Ví dụ: "Đỏ", "Xanh")
    public class FilterOptionDto
    {
        public string Label { get; set; } = string.Empty; // "Đỏ"
        public string Value { get; set; } = string.Empty; // "red" (hoặc ID)
        public int Count { get; set; } // Số lượng sản phẩm có thuộc tính này (15)
        public bool IsActive { get; set; } // Frontend tự xử lý, hoặc BE trả về nếu cần
    }
}
