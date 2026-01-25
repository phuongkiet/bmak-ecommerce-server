using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommand : IRequest<int>
    {
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public decimal BasePrice { get; set; } // Giá bán thường (giá gốc)
        public decimal SalePrice { get; set; } // Giá giảm (nếu có giảm giá)
        public string SalesUnit { get; set; } = string.Empty;  // "Thùng", "Hộp"
        public string? PriceUnit { get; set; }  // "m2", "Viên" - optional
        public float ConversionFactor { get; set; }
        public int CategoryId { get; set; }

        public bool? AllowBackorder { get; set; } // Default: true
        public bool? ManageStock { get; set; }    // Default: true

        public float? InitialStock { get; set; }
        public string? WarehouseName { get; set; }

        // Thông tin giảm giá (optional)
        public DateTime? SaleStartDate { get; set; } // Ngày bắt đầu giảm giá
        public DateTime? SaleEndDate { get; set; } // Ngày kết thúc giảm giá

        // Optional fields - nếu không có thì để null
        public float? Weight { get; set; } // Cân nặng (kg) để tính ship
        public string? ImageUrl { get; set; } // URL ảnh sản phẩm
        public string? SpecificationsJson { get; set; } // JSON specifications - có thể là JSON string hoặc null
        public bool? IsActive { get; set; } // Trạng thái active - default true nếu null

        // List thuộc tính (nếu tạo luôn lúc add sản phẩm)
        public List<CreateProductAttributeRequest>? Attributes { get; set; }

        // List Tag IDs (nếu muốn gán tags cho sản phẩm)
        public List<int>? TagIds { get; set; }
    }
}
