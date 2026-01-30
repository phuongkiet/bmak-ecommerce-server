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
        public string? ImageUrl { get; set; }
        public string? SpecificationsJson { get; set; }
        public bool? IsActive { get; set; }

        // --- CẤU HÌNH KHO (MỚI) ---
        public bool? AllowBackorder { get; set; } // Cho phép bán âm?
        public bool? ManageStock { get; set; }    // Có quản lý kho không?
        public float? InitialStock { get; set; }  // Số lượng nhập kho ban đầu
        public string? WarehouseName { get; set; }

        public List<CreateProductAttributeRequest>? Attributes { get; set; }
        public List<int>? TagIds { get; set; }
    }
}
