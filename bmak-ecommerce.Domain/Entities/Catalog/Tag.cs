using bmak_ecommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Entities.Catalog
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; } // VD: "Bán chạy", "Mới", "Khuyến mãi", "Cao cấp"
        public string Slug { get; set; } // URL-friendly: "ban-chay", "moi", "khuyen-mai"
        public string? Description { get; set; } // Mô tả (optional)

        // Navigation property: Many-to-Many với Product
        public virtual ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
    }
}


