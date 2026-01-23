using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.ProductAttributeValues.Commands.CreateProductAttributeValue
{
    public class CreateProductAttributeValueCommand : IRequest<int>
    {
        public string Value { get; set; } = string.Empty; // VD: "60x60", "Xám", "Matt"
        public string? ExtraData { get; set; } // Optional: Mã màu Hex (#FF5733), Icon URL, hoặc metadata khác
        
        public int ProductId { get; set; } // ID của Product
        public int AttributeId { get; set; } // ID của ProductAttribute
    }
}


