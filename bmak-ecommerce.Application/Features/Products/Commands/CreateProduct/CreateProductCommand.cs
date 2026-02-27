using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommand
    {
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string Slug { get; set; }
        public string? ShortDescription { get; set; } // Text thường
        public string? Description { get; set; }

        // Giá & Đơn vị
        public decimal BasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public string SalesUnit { get; set; } = string.Empty;   // "Viên"
        public string? PriceUnit { get; set; }                  // "m2"
        public float ConversionFactor { get; set; }             // 0.36

        public int CategoryId { get; set; }
        public DateTime? SaleStartDate { get; set; }
        public DateTime? SaleEndDate { get; set; }

        public float? Weight { get; set; }
        public string? ThumbnailUrl { get; set; } // Ảnh đại diện
        public List<int>? ImageIds { get; set; }
        public string? SpecificationsJson { get; set; }
        public bool? IsActive { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public decimal? Thickness { get; set; }
        public int? Random { get; set; }
        public int? BoxQuantity { get; set; }

        // --- CẤU HÌNH KHO (MỚI) ---
        public bool? AllowBackorder { get; set; } // Cho phép bán âm?
        public bool? ManageStock { get; set; }    // Có quản lý kho không?
        public float? InitialStock { get; set; }  // Số lượng nhập kho ban đầu
        public string? WarehouseName { get; set; }

        public List<CreateProductAttributeRequest>? Attributes { get; set; }
        public List<int>? TagIds { get; set; }
        public List<int> CategoryIds { get; set; } = new List<int>();
    }
}
