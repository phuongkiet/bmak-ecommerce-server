using bmak_ecommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Entities.Catalog
{
    public class ProductImage : BaseEntity
    {
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMain { get; set; } // Đánh dấu đây là ảnh chính (Thumbnail)
        public int SortOrder { get; set; } // Thứ tự hiển thị

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
