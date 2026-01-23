using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Products.DTOs.Catalog
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        // Thông tin danh mục cha (nếu có)
        public int? ParentId { get; set; }
        public string? ParentName { get; set; }
        
        // Số lượng sản phẩm trong danh mục
        public int ProductCount { get; set; }
    }
}



