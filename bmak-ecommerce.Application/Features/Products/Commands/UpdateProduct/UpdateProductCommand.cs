using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommand
    {
        [JsonIgnore]
        public int Id { get; set; } // Sẽ lấy từ URL (Route) gán vào đây

        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;

        // Giá & Đơn vị
        public decimal BasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public string SalesUnit { get; set; } = string.Empty;
        public string? PriceUnit { get; set; }
        public float ConversionFactor { get; set; }

        public int CategoryId { get; set; }

        public DateTime? SaleStartDate { get; set; }
        public DateTime? SaleEndDate { get; set; }

        public float? Weight { get; set; }
        public string? ImageUrl { get; set; }
        public string? SpecificationsJson { get; set; }
        public bool? IsActive { get; set; }

        // Attributes
        // Lưu ý: Dùng đúng tên class DTO bạn định nghĩa bên dưới
        public List<UpdateProductAttributeDto> Attributes { get; set; } = new();

        // Tags
        public List<int> TagIds { get; set; } = new();

        // Bổ sung các field cấu hình kho (nếu bạn muốn Update cả phần này)
        // public bool? AllowBackorder { get; set; } 
        // public bool? ManageStock { get; set; }
    }

    public class UpdateProductAttributeDto
    {
        public int AttributeId { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}

