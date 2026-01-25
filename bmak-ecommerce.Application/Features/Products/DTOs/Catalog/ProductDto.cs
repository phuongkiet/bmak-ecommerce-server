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
		public string Slug { get; set; }
		public string Sku { get; set; }
		public string? ShortDescription { get; set; }
		public string? Description { get; set; }
		public string? SpecificationsJson { get; set; } // JSON thông số kỹ thuật
		public string SalesUnit { get; set; }  // "Thùng", "Viên"
		public string PriceUnit { get; set; }  // "m2", "Viên"
		public float ConversionFactor { get; set; } // Hệ số quy đổi (VD: 0.36)

		// Giá & Kho
		public decimal Price { get; set; }       // SalePrice
		public decimal? OriginalPrice { get; set; } // BasePrice
		public int StockQuantity { get; set; }

		// Hình ảnh
		public string? Thumbnail { get; set; }
		public List<ProductImageDto> Images { get; set; } = new();

		// Thuộc tính (Để hiển thị: Màu Titan, RAM 8GB...)
		public List<ProductAttributeDto> Attributes { get; set; } = new();

		// Danh mục
		public int CategoryId { get; set; }
		public string CategoryName { get; set; }
	}
}
