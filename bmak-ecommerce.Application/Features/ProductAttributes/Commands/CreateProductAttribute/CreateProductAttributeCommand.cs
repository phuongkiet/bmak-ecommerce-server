using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.ProductAttributes.Commands.CreateProductAttribute
{
    public class CreateProductAttributeCommand : IRequest<int>
    {
        public string Name { get; set; } = string.Empty; // VD: "Kích thước", "Màu sắc"
        public string Code { get; set; } = string.Empty; // VD: "SIZE", "COLOR" (thường là UPPERCASE)
    }
}


