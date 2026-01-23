using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Products.DTOs.Catalog
{
    /// <summary>
    /// DTO cho việc list ProductAttributes (dùng khi GET all attributes để user chọn)
    /// </summary>
    public class ProductAttributeListItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } // VD: "Kích thước", "Màu sắc"
        public string Code { get; set; } // VD: "SIZE", "COLOR"
    }
}


