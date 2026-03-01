using bmak_ecommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.ProductAttributeValues.DTOs
{
    public class ProductAttributeValueDto : BaseEntity
    {
        public string Value { get; set; } // "60x60", "Matt"
        public string? ExtraData { get; set; } // Mã màu Hex hoặc Icon

        public int AttributeId { get; set; }
    }
}
