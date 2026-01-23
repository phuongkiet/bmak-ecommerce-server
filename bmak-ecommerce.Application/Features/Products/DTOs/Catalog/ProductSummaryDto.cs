using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Products.DTOs.Catalog
{
    public class ProductSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty; // Để tạo link SEO
        public string Sku { get; set; } = string.Empty;

        // --- Pricing (Xử lý hiển thị giá) ---
        public decimal Price { get; set; }       // Giá bán thực tế (đã giảm)
        public decimal? OriginalPrice { get; set; } // Giá gốc (để gạch ngang)
        public int DiscountPercentage => OriginalPrice.HasValue && OriginalPrice > Price
            ? (int)Math.Round((1 - (Price / OriginalPrice.Value)) * 100)
            : 0;

        // --- Visual ---
        public string Thumbnail { get; set; } = string.Empty; // Ảnh đại diện

        // --- Badges (Để FE hiển thị nhãn) ---
        // Thay vì trả về list object tags, chỉ trả về mảng string đơn giản
        public List<string> Badges { get; set; } = new(); // Ví dụ: ["Mới", "Bán chạy", "-20%"]

        // --- Key Specs (Quan trọng) ---
        // Chỉ map 1-2 thông số quan trọng nhất để khách nhận diện nhanh
        // Ví dụ: Laptop thì hiện "RAM 8GB", Gạch thì hiện "60x60"
        // Dạng Dictionary hoặc List string cho gọn
        public Dictionary<string, string> KeySpecs { get; set; } = new();

        // --- Social Proof ---
        public double Rating { get; set; } // 4.5
        public int ReviewCount { get; set; } // (120)
        public int TotalSold { get; set; } // Đã bán 1k
    }
}
