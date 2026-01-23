using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Products.DTOs.Catalog
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SKU { get; set; }
        public string Slug { get; set; }
        public decimal BasePrice { get; set; } // Giá bán thường
        public decimal SalePrice { get; set; } // Giá giảm
        public int TotalSold { get; set; }
        public DateTime? SaleStartDate { get; set; } // Ngày bắt đầu giảm giá
        public DateTime? SaleEndDate { get; set; } // Ngày kết thúc giảm giá
        public string ImageUrl { get; set; } // Giả sử bạn có ảnh

        // Flatten category (Chỉ lấy tên cho gọn)
        public string CategoryName { get; set; }
        public string CategorySlug { get; set; }

        // List thuộc tính để hiển thị (VD: Kích thước: 60x60, Màu: Xám)
        public List<ProductAttributeDto> Attributes { get; set; }

        // List tags (VD: "Bán chạy", "Mới", "Khuyến mãi")
        public List<TagDto> Tags { get; set; }
    }
}
