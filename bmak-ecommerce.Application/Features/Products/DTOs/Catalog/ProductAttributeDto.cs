using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Products.DTOs.Catalog
{
    public class ProductAttributeDto
    {
        public int AttributeId { get; set; }
        public string Name { get; set; } // Tên thuộc tính (Màu sắc)
        public string Value { get; set; } // Giá trị (Xám)
        public string Code { get; set; } // COLOR
    }
}
