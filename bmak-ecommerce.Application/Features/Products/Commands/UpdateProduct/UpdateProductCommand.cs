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
    public class UpdateProductCommand : IRequest<bool>
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public decimal BasePrice { get; set; } // Giá bán thường (giá gốc)
        public decimal SalePrice { get; set; } // Giá giảm (nếu có giảm giá)
        public string SalesUnit { get; set; } = string.Empty;
        public string? PriceUnit { get; set; }
        public float ConversionFactor { get; set; }
        public int CategoryId { get; set; }

        // Thông tin giảm giá (optional)
        public DateTime? SaleStartDate { get; set; } // Ngày bắt đầu giảm giá
        public DateTime? SaleEndDate { get; set; } // Ngày kết thúc giảm giá

        // Optional fields
        public float? Weight { get; set; }
        public string? ImageUrl { get; set; }
        public string? SpecificationsJson { get; set; }
        public bool? IsActive { get; set; }

        // Attributes và Tags - khi update có thể thay đổi
        public List<ProductAttributeDto> Attributes { get; set; } = new();
        public List<int> TagIds { get; set; } = new();
    }

    public class UpdateProductAttributeDto
    {
        public int AttributeId { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}

